import { Logger } from "@aws-lambda-powertools/logger";
import "dotenv/config";
import { createCronJobService } from "./cronJobService.mts";
import { IJobService } from "./iJobService.mts";

const logger: Logger = new Logger({ serviceName: "schedulerService" });

const main = async () => {
  const jobService: IJobService = createCronJobService(logger);
  await jobService.execute();
};

await main();
