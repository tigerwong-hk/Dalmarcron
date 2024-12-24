import { Logger } from "@aws-lambda-powertools/logger";
import { getParametersByName } from "@aws-lambda-powertools/parameters/ssm";
import type { SSMGetParametersByNameOptions } from "@aws-lambda-powertools/parameters/ssm/types";
import { OAuth2Client, OAuth2Fetch } from "@badgateway/oauth2-client";
import { IApiService } from "./iApiService.mts";

let oauth2FetchClient: OAuth2Fetch | null;

async function createOauth2HttpClient(logger: Logger): Promise<OAuth2Fetch> {
  const oauth2BaseUri: string = process.env["OAUTH2_BASE_URI"] ?? "";
  if (oauth2BaseUri.trim().length < 1) {
    throw new Error("Oauth2 base URI missing");
  }

  const oauth2ClientScope: string = process.env["OAUTH2_CLIENT_SCOPE"] ?? "";
  if (oauth2ClientScope.trim().length < 1) {
    throw new Error("Oauth2 client scope missing");
  }
  const clientScopes: string[] = oauth2ClientScope.split(",");

  for (let i = 0; i < clientScopes.length; i++) {
    console.log(`clientScopes[${i.toString()}]: `, clientScopes[i]);
  }

  const ssmParametersPathPrefix: string = process.env["SSM_PARAMETERS_PATH_PREFIX"] ?? "";
  if (ssmParametersPathPrefix.trim().length < 1) {
    throw new Error("SSM parameters path prefix missing");
  }

  const oauth2ParametersOptions: Record<string, SSMGetParametersByNameOptions> = {
    [`${ssmParametersPathPrefix}/Oauth2ClientId`]: { decrypt: true },
    [`${ssmParametersPathPrefix}/Oauth2ClientSecret`]: { decrypt: true },
  };

  const oauth2Parameters: Record<string, string> =
    await getParametersByName<string>(oauth2ParametersOptions);
  /*
  for (const [oauth2Key, oauth2Value] of Object.entries(oauth2Parameters)) {
    console.log(`${oauth2Key}: ${oauth2Value}`);
  }
  */

  const oauth2ClientId: string =
    oauth2Parameters[`${ssmParametersPathPrefix}/Oauth2ClientId`] ?? "";
  if (oauth2ClientId.trim().length < 1) {
    throw new Error("Oauth2 client ID missing");
  }

  const oauth2ClientSecret: string =
    oauth2Parameters[`${ssmParametersPathPrefix}/Oauth2ClientSecret`] ?? "";
  if (oauth2ClientSecret.trim().length < 1) {
    throw new Error("Oauth2 client secret missing");
  }

  const oauth2Client = new OAuth2Client({
    server: oauth2BaseUri,
    clientId: oauth2ClientId,
    clientSecret: oauth2ClientSecret,
  });

  return new OAuth2Fetch({
    client: oauth2Client,
    getNewToken: async () => {
      return await oauth2Client.clientCredentials({ scope: clientScopes });
    },
    onError: (err: Error) => {
      logger.error("OAuth2Fetch error", err);
    },
    scheduleRefresh: false,
  });
}

async function getOauth2HttpClient(logger: Logger): Promise<OAuth2Fetch> {
  if (!oauth2FetchClient) {
    oauth2FetchClient = await createOauth2HttpClient(logger);
  }

  return oauth2FetchClient;
}

const send =
  (logger: Logger) =>
  async (input: RequestInfo, init?: RequestInit): Promise<Response> => {
    const httpClient: OAuth2Fetch = await getOauth2HttpClient(logger);

    try {
      const response: Response = await httpClient.fetch(input, init);
      if (!response.ok) {
        throw new Error(`Response status: ${response.status.toString()} ${response.statusText}`);
      }

      logger.info("OAuth2 Fetch API response ok", response.ok.toString());
      logger.info("OAuth2 Fetch API response status", response.status.toString());
      logger.info("OAuth2 Fetch API response status text", response.statusText);
      logger.info("OAuth2 Fetch API response type", response.type);
      logger.info("OAuth2 Fetch API response url", response.url);
      logger.info("OAuth2 Fetch API response redirected", response.redirected.toString());

      return response;
    } catch (err) {
      logger.error("OAuth2 Fetch exception", err instanceof Error ? err : JSON.stringify(err));
      throw err;
    }
  };

export const createOauth2ApiService = (logger: Logger): IApiService => {
  return {
    send: send(logger),
  };
};
