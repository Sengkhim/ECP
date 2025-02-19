import { Module } from '@nestjs/common';
import {ConfigModule} from '@nestjs/config';
import {UserModule} from "@modules/user/user.module";
import {PrismaModule} from "@orm/prisma.module";
import {JwtModule} from "@nestjs/jwt";
import {AuthModule} from "@modules/auth/auth.module";
import {ApplicationModule} from "@app/services/application.module";
import * as process from 'node:process';


@Module({
    imports: [
        ConfigModule.forRoot({
            isGlobal: true
        }),
        JwtModule.register({
            secret: process.env.JWT_SECRET,
            signOptions: { expiresIn: process.env.EXPIRES_IN },
        }),
        ApplicationModule,
        ConfigModule,
        PrismaModule,
        UserModule,
        AuthModule,
    ]
})
export class AppModule {}
