export interface IApiService {
  fetch(input: RequestInfo, init?: RequestInit): Promise<Response>;
}
