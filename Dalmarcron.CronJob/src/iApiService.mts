export interface IApiService {
  send(input: RequestInfo, init?: RequestInit): Promise<Response>;
}
