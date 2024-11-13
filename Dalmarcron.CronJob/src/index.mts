import type { Context, EventBridgeEvent, EventBridgeHandler } from "aws-lambda";

export const lambdaHandler: EventBridgeHandler<"Scheduled Event", null, void> = async (event: EventBridgeEvent<"Scheduled Event", null>, context: Context): Promise<void>  => {
  console.log("Event", JSON.stringify(event, null, 2));
  console.log("Context", JSON.stringify(context, null, 2));
};

