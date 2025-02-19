import { AuthProviderDto } from '@modules/auth/dto/auth-provider.dto';
import { IResponse } from '@wrapper/inteface/response';
import { OAuthProvider } from '@prisma/client';

export interface IAuthProviderService {

    create(request: AuthProviderDto): Promise<IResponse<OAuthProvider>>;
}