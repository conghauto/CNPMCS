import { Component, ElementRef, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AccountServiceProxy, IsTenantAvailableInput, IsTenantAvailableOutput, TenantAvailabilityState } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'tenantChangeModal',
    templateUrl: './tenant-change-modal.component.html'
})
export class TenantChangeModalComponent extends AppComponentBase {

    @ViewChild('tenantChangeModal') modal: ModalDirective;
    @ViewChild('tenancyNameInput') tenancyNameInput: ElementRef;

    tenancyName = '';
    active = false;
    saving = false;

    constructor(
        private _accountService: AccountServiceProxy,
        injector: Injector
    ) {
        super(injector);
    }

    show(tenancyName: string): void {
        this.tenancyName = tenancyName;
        this.active = true;
        this.modal.show();
    }

    onShown(): void {
        (this.tenancyNameInput.nativeElement as any).focus();
    }

    save(): void {

        if (!this.tenancyName) {
            abp.multiTenancy.setTenantIdCookie(undefined);
            this.close();
            location.reload();
            return;
        }

        let input = new IsTenantAvailableInput();
        input.tenancyName = this.tenancyName;

        this.saving = true;
        this._accountService.isTenantAvailable(input)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe((result: IsTenantAvailableOutput) => {
                switch (result.state) {
                    case TenantAvailabilityState.Available:
                        abp.multiTenancy.setTenantIdCookie(result.tenantId);
                        this.close();
                        location.reload();
                        return;
                    case TenantAvailabilityState.InActive:
                        this.message.warn(this.l('TenantIsNotActive', this.tenancyName));
                        break;
                    case TenantAvailabilityState.NotFound: //NotFound
                        this.message.warn(this.l('ThereIsNoTenantDefinedWithName{0}', this.tenancyName));
                        break;
                }
            });
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
