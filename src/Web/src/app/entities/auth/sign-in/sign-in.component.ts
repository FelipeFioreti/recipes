import {HttpErrorResponse} from '@angular/common/http';
import {ChangeDetectionStrategy, Component, signal} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {Router, RouterLink} from '@angular/router';
import {finalize} from 'rxjs';
import {AuthService} from "../../../core/services/auth.service";
import {ApiErrorResponse} from "../../../core/models/api-error.model";

function extractErrorMessage(error: HttpErrorResponse): string {
    const payload = error.error as ApiErrorResponse | null;

    return payload?.message ?? 'Nao foi possivel autenticar com as credenciais informadas.';
}

@Component({
    selector: 'app-sign-in',
    standalone: true,
    imports: [ReactiveFormsModule, RouterLink],
    templateUrl: './sign-in.component.html',
    styleUrls: ['./sign-in.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class SignInComponent {

    readonly isSubmitting = signal(false);
    readonly errorMessage = signal('');
    readonly form = this.formBuilder.nonNullable.group({
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]]
    });

    public constructor(
        private authService: AuthService,
        private formBuilder: FormBuilder,
        private router: Router) {
    }

    submit(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting.set(true);
        this.errorMessage.set('');

        this.authService
            .signIn(this.form.getRawValue())
            .pipe(finalize(() => this.isSubmitting.set(false)))
            .subscribe({
                next: () => {
                    void this.router.navigateByUrl('/app/home');
                },
                error: (error: HttpErrorResponse) => {
                    this.errorMessage.set(extractErrorMessage(error));
                }
            });
    }
}

