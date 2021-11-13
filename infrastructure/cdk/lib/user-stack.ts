import * as path from 'path';
import * as cdk from '@aws-cdk/core';
import * as ec2 from '@aws-cdk/aws-ec2';
import * as ecr from '@aws-cdk/aws-ecr';
import * as ecs from '@aws-cdk/aws-ecs';
import { DockerImageAsset } from '@aws-cdk/aws-ecr-assets';
import * as ecr_deployment from "cdk-ecr-deployment";
import { ApiService } from './api-service';
import * as elbv2 from '@aws-cdk/aws-elasticloadbalancingv2';
import { MySqlConstruct } from '@cloud-platform/infrastructure-core/lib/mysql-construct';

export type UserStackProps = {
  vpc: ec2.Vpc,
  nginxImage: DockerImageAsset,
  instanceClass: ec2.InstanceClass;
  instanceSize: ec2.InstanceSize;
  loadBalancer: elbv2.ApplicationLoadBalancer,
  httpListener: elbv2.ApplicationListener
} & cdk.StackProps;

export class UserStack extends cdk.Stack {

  readonly cluster: ecs.Cluster;

  constructor(scope: cdk.Construct, id: string, props: UserStackProps) {
    super(scope, id, props);

    const serviceName = 'UserService';

    const serviceRepo = new ecr.Repository(this, `${serviceName}-repo`, {
      removalPolicy: cdk.RemovalPolicy.DESTROY,
      imageScanOnPush: true,
      repositoryName: serviceName.toLocaleLowerCase(),
      lifecycleRules: [{
        maxImageCount: 3
      }]
    });

    const apiImage = new DockerImageAsset(this, 'User-Api', {
      directory: path.join(__dirname, '../../../Api')
    });

    new ecr_deployment.ECRDeployment(this, "DeployImageLatestTag", {
      src: new ecr_deployment.DockerImageName(apiImage.imageUri),
      dest: new ecr_deployment.DockerImageName(`${serviceRepo.repositoryUri}:latest`),
      vpc: props.vpc,
    });

    this.cluster = new ecs.Cluster(this, `${serviceName}-Cluster`, {
      clusterName: `${serviceName}-Cluster`,
      vpc: props.vpc
    });

    const autoScalingGroup = this.cluster.addCapacity('DefaultAutoScalingGroup', {
      instanceType: ec2.InstanceType.of(props.instanceClass, props.instanceSize),
    });

    props.httpListener.addTargets("addTargetGrouop", {
      port: 80,
      targets: [
        autoScalingGroup
      ]
    });
    
    autoScalingGroup.connections.allowFrom(
      props.loadBalancer,
      ec2.Port.tcp(80),
      "ALB access 80 port of EC2 in Autoscaling Group");

    const rds = new MySqlConstruct(this, 'MySql', {
      vpc: props.vpc,
      instanceClass: ec2.InstanceClass.T4G,
      instanceSize: ec2.InstanceSize.NANO,
      asgSecurityGroups: autoScalingGroup.connections.securityGroups
    })

    const service = new ApiService(this, `${serviceName}-Service`, {
      stack: this,
      nginxImage: props.nginxImage,
      apiRepository: serviceRepo,
      serviceName,
      cluster: this.cluster,
      cpu: 128,
      memoryLimitMiB: 128,
      vpc: props.vpc,
      autoScalingGroup
    });

    service.albFargate.targetGroup.configureHealthCheck({
      path: '/health',
      healthyThresholdCount: 2,
      unhealthyThresholdCount: 5,
      interval: cdk.Duration.seconds(60),
      timeout: cdk.Duration.seconds(15)
    });
  }
}
