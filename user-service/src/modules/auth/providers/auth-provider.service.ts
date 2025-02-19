import {PrismaService} from "@orm/prisma.service";
import {AuthProviderDto} from "@modules/auth/dto/auth-provider.dto";
import {OAuthProvider} from "@prisma/client";
import {ResponseImpl} from "@wrapper/imeplement/response.implement";
import { IAuthProviderService } from '@modules/auth/providers/service/auth-provider.service';

export class AuthProviderService implements IAuthProviderService {
    
    constructor(private readonly prismaService: PrismaService) {}
    
    async create(request: AuthProviderDto) {
        const data: OAuthProvider = await this
            .prismaService
            .oAuthProvider
            .create({
                data: request
            });
        return ResponseImpl.success(data);
    }
}
