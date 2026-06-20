import { Component, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  registerForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder, 
    private auth: AuthService, 
    private router: Router,
    private cdr: ChangeDetectorRef
    ) {
    this.registerForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) return;

    this.isLoading = true;
    const payload = this.registerForm.value;

    this.auth.register(this.registerForm.value).subscribe({
      next: () => this.router.navigate(['/login']),
      error: (err) => {
        this.errorMessage = err.error?.message || 'Registration failed';
        this.isLoading = false;

        const errorMessage = err.error?.message || 'Registration failed. Please try again.';
        alert(errorMessage);
        this.cdr.detectChanges();
      }
    });
  }

  get f() { return this.registerForm.controls; }
}