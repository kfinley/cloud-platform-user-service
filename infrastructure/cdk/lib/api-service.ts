import * as cdk from '@aws-cdk/core';
import * as ecs from '@aws-cdk/aws-ecs';
import * as ecr from '@aws-cdk/aws-ecr';
import * as ec2 from '@aws-cdk/aws-ec2';
import * as logs from '@aws-cdk/aws-logs';
import * as autoscaling from '@aws-cdk/aws-autoscaling';
import { DockerImageAsset } from '@aws-cdk/aws-ecr-assets';
import * as iam from '@aws-cdk/aws-iam';
import { LoadBalancedFargate, LoadBalancedFargateProps } from '@cloud-platform/infrastructure-core/lib/alb-fargate';

export type ApiServiceProps = {
  nginxImage: DockerImageAsset,
  apiRepository: ecr.Repository
  stack: cdk.Stack
  vpc: ec2.Vpc,
  autoScalingGroup: autoscaling.AutoScalingGroup
} & LoadBalancedFargateProps;

export class ApiService extends LoadBalancedFargate {

  constructor(scope: cdk.Stack, id: string, props: ApiServiceProps) {
    super(scope, id, props);
  }

  protected createFargateTaskDefinition(): ecs.FargateTaskDefinition {

    const taskRole = new iam.Role(this, "TaskRole", {
      assumedBy: new iam.ServicePrincipal("ecs-tasks.amazonaws.com")
    });

    const taskSecGroup = new ec2.SecurityGroup(this, "TaskSecurityGroup", {
      vpc: (<ApiServiceProps>this.props).vpc
    });

    (<ApiServiceProps>this.props).autoScalingGroup.addSecurityGroup(taskSecGroup);

    // taskSecGroup.addIngressRule(ec2.Peer.anyIpv4(), ec2.Port.tcp(80));

    const taskDefinition = new ecs.FargateTaskDefinition(this, `${this.props.serviceName}-FargateTaskDefinition`, {
      cpu: this.props.cpu,
      memoryLimitMiB: this.props.memoryLimitMiB,
      taskRole
    });

    taskDefinition
      .addContainer(`${this.props.serviceName}-api`, {
        image: ecs.ContainerImage.fromEcrRepository((<ApiServiceProps>this.props).apiRepository, 'latest'),
        containerName: `${this.props.serviceName}-api`,
        logging: ecs.LogDrivers.awsLogs({
          streamPrefix: this.props.serviceName,
          logGroup: new logs.LogGroup(this, "API", {
            retention: logs.RetentionDays.FIVE_DAYS,
          }),
        }),
      }).addPortMappings({
        containerPort: 5000
      });

    taskDefinition.addContainer(`${this.props.serviceName}-nginx`, {
      image: ecs.AssetImage.fromEcrRepository((<ApiServiceProps>this.props).nginxImage.repository, 'latest'),
      containerName:`${this.props.serviceName}-nginx`,
      logging: ecs.LogDrivers.awsLogs({
        streamPrefix: this.props.serviceName,
        logGroup: new logs.LogGroup(this, "Nginx", {
          retention: logs.RetentionDays.FIVE_DAYS,
        }),
      }),
    }).addPortMappings({
      containerPort: 80
    });

    return taskDefinition;
  }
}