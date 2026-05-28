import {HttpErrorResponse} from '@angular/common/http';
import {ChangeDetectionStrategy, Component, signal} from '@angular/core';
import {AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators} from '@angular/forms';
import {Router, RouterLink} from '@angular/router';
import {finalize} from 'rxjs';
import {ApiErrorResponse} from "../../../core/models/api-error.model";
import {NotificationService} from "../../../core/services/notification.service";
import {AuthService} from "../../../core/services/auth.service";

function passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;

    return password && confirmPassword && password !== confirmPassword ? {passwordMismatch: true} : null;
}

function extractErrorMessage(error: HttpErrorResponse): string {
    const payload = error.error as ApiErrorResponse | null;

    return payload?.message ?? 'Nao foi possivel concluir o cadastro.';
}

@Component({
    selector: 'app-sign-up',
    standalone: true,
    imports: [ReactiveFormsModule, RouterLink],
    templateUrl: './sign-up.component.html',
    styleUrls: ['./sign-up.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class SignUpComponent {

    readonly isSubmitting = signal(false);
    readonly errorMessage = signal('');
    readonly form = this.formBuilder.nonNullable.group(
        {
            name: ['', [Validators.required, Validators.maxLength(255)]],
            email: ['', [Validators.required, Validators.email]],
            password: ['', [Validators.required, Validators.minLength(6)]],
            confirmPassword: ['', [Validators.required]]
        },
        {validators: passwordMatchValidator}
    );

    public constructor(
        private notificationService: NotificationService,
        private authService: AuthService,
        private formBuilder: FormBuilder,
        private router: Router,
    ) {
    }

    submit(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        const {confirmPassword: _, ...payload} = this.form.getRawValue();

        this.isSubmitting.set(true);
        this.errorMessage.set('');

        this.authService
            .register(payload)
            .pipe(finalize(() => this.isSubmitting.set(false)))
            .subscribe({
                next: () => {
                    this.notificationService.success('Conta criada', 'Agora voce pode entrar e comecar a catalogar receitas.');
                    void this.router.navigateByUrl('/auth/login');
                },
                error: (error: HttpErrorResponse) => {
                    this.errorMessage.set(extractErrorMessage(error));
                }
            });
    }
}

