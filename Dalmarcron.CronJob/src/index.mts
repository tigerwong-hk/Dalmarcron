import type { Context, EventBridgeEvent, EventBridgeHandler } from "aws-lambda";
import { Logger } from "@aws-lambda-powertools/logger";
import { createCronJobService } from "./cronJobService.mts";
import { IJobService } from "./iJobService.mts";

const logger: Logger = new Logger({ serviceName: "schedulerService" });

export const lambdaHandler: EventBridgeHandler<"Scheduled Event", null, void> = async (
  event: EventBridgeEvent<"Scheduled Event", null>,
  context: Context,
): Promise<void> => {
  logger.info("Event", JSON.stringify(event));
  logger.info("Context", JSON.stringify(context));

  const jobService: IJobService = createCronJobService(logger);
  await jobService.execute();
};
