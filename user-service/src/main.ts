import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';
import {INestApplication} from '@nestjs/common';
import {ApplicationService} from "@app/services/application.service";
import * as session from 'express-session';
import * as passport from 'passport';

async function startup() {
    const builder: INestApplication = await NestFactory.create(AppModule);
    const app: ApplicationService = builder.get(ApplicationService);
    builder.setGlobalPrefix("api");
    builder.use(
        session({
            secret: app.getJwtSecret(), 
            resave: false,
            saveUninitialized: false,
            cookie: {
                secure: app.getAppMode() === 'production',
                httpOnly: true,
                maxAge: 1000 * 60 * 60 * 24, // 1 day
            },
        }),
    );
    builder.use(passport.initialize());
    builder.use(passport.session());
    
    app.useValidate(builder);
    app.useSwagger(builder);
    await app.run(builder);
}

void startup();
