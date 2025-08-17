import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-search-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './search-form.component.html',
  styleUrls: ['./search-form.component.css'],
})
export class SearchFormComponent implements OnInit {
  @Output() searchRequested = new EventEmitter<string>();

  searchForm!: FormGroup;
  isSubmitting = false;
  submitted = false;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.initForm();
  }

  /**
   * Initialize the reactive form with validation
   */
  private initForm(): void {
    this.searchForm = this.fb.group({
      zipCode: [
        '',
        [
          Validators.required,
          Validators.pattern(/^\d{5}$/),
          Validators.minLength(5),
          Validators.maxLength(5),
        ],
      ],
    });
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    this.submitted = true;

    if (this.searchForm.valid) {
      this.isSubmitting = true;
      const zipCode = this.searchForm.get('zipCode')?.value?.trim();

      // Emit the search request
      this.searchRequested.emit(zipCode);

      // Reset submitting state after a short delay
      setTimeout(() => {
        this.isSubmitting = false;
      }, 1000);
    }
  }

  /**
   * Handle Enter key press in the input field
   */
  onKeyPress(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      this.onSubmit();
    }
  }

  /**
   * Get the zip code form control for easy access in template
   */
  get zipCodeControl() {
    return this.searchForm.get('zipCode');
  }

  /**
   * Check if the zip code field has validation errors
   */
  hasZipCodeError(): boolean {
    const control = this.zipCodeControl;
    return this.submitted && control ? control.invalid || false : false;
  }

  /**
   * Get the error message for the zip code field
   */
  getZipCodeErrorMessage(): string {
    const control = this.zipCodeControl;
    if (!control || !control.errors) return '';

    if (control.errors['required']) {
      return 'Zip code is required';
    }
    if (control.errors['pattern']) {
      return 'Please enter a valid 5-digit zip code';
    }
    if (control.errors['minlength'] || control.errors['maxlength']) {
      return 'Zip code must be exactly 5 digits';
    }

    return 'Please enter a valid zip code';
  }

  /**
   * Reset the form and validation state
   */
  resetForm(): void {
    this.searchForm.reset();
    this.submitted = false;
    this.isSubmitting = false;
  }
}
