import {Global, Module} from "@nestjs/common";
import {ApplicationService} from "@app/services/application.service";

@Global()
@Module({
    providers: [ApplicationService],
    exports: [ApplicationService],
})
export class ApplicationModule {}