import type { Context, EventBridgeEvent, EventBridgeHandler } from "aws-lambda";
import { Logger } from "@aws-lambda-powertools/logger";

const logger: Logger = new Logger();

export const lambdaHandler: EventBridgeHandler<"Scheduled Event", null, void> = async (
  event: EventBridgeEvent<"Scheduled Event", null>,
  context: Context,
  // eslint-disable-next-line @typescript-eslint/require-await
): Promise<void> => {
  logger.info(`Event: ${JSON.stringify(event, null, 2)}`);
  logger.info(`Context: ${JSON.stringify(context, null, 2)}`);
};
