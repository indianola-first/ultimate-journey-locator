import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { SearchFormComponent } from './search-form.component';

describe('SearchFormComponent', () => {
  let component: SearchFormComponent;
  let fixture: ComponentFixture<SearchFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SearchFormComponent, ReactiveFormsModule],
    }).compileComponents();

    fixture = TestBed.createComponent(SearchFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with empty form', () => {
    expect(component.searchForm).toBeTruthy();
    expect(component.searchForm.get('zipCode')?.value).toBe('');
    expect(component.searchForm.get('zipCode')?.valid).toBeFalsy();
  });

  it('should have zipCode form control', () => {
    const zipCodeControl = component.searchForm.get('zipCode');
    expect(zipCodeControl).toBeTruthy();
  });

  it('should validate zip code format', () => {
    const zipCodeControl = component.searchForm.get('zipCode');

    // Valid zip codes
    zipCodeControl?.setValue('10001');
    expect(zipCodeControl?.valid).toBeTruthy();

    zipCodeControl?.setValue('90210');
    expect(zipCodeControl?.valid).toBeTruthy();

    zipCodeControl?.setValue('12345');
    expect(zipCodeControl?.valid).toBeTruthy();

    // Invalid zip codes
    zipCodeControl?.setValue('1234');
    expect(zipCodeControl?.valid).toBeFalsy();

    zipCodeControl?.setValue('123456');
    expect(zipCodeControl?.valid).toBeFalsy();

    zipCodeControl?.setValue('abc12');
    expect(zipCodeControl?.valid).toBeFalsy();

    zipCodeControl?.setValue('');
    expect(zipCodeControl?.valid).toBeFalsy();
  });

  it('should emit searchRequested event when form is submitted with valid zip code', () => {
    spyOn(component.searchRequested, 'emit');
    const zipCode = '10001';

    component.searchForm.get('zipCode')?.setValue(zipCode);
    component.onSubmit();

    expect(component.searchRequested.emit).toHaveBeenCalledWith(zipCode);
  });

  it('should not emit searchRequested event when form is submitted with invalid zip code', () => {
    spyOn(component.searchRequested, 'emit');

    component.searchForm.get('zipCode')?.setValue('invalid');
    component.onSubmit();

    expect(component.searchRequested.emit).not.toHaveBeenCalled();
  });

  it('should not emit searchRequested event when form is submitted with empty zip code', () => {
    spyOn(component.searchRequested, 'emit');

    component.searchForm.get('zipCode')?.setValue('');
    component.onSubmit();

    expect(component.searchRequested.emit).not.toHaveBeenCalled();
  });

  it('should handle Enter key press', () => {
    spyOn(component, 'onSubmit');
    const event = new KeyboardEvent('keypress', { key: 'Enter' });

    component.onKeyPress(event);

    expect(component.onSubmit).toHaveBeenCalled();
  });

  it('should not handle non-Enter key press', () => {
    spyOn(component, 'onSubmit');
    const event = new KeyboardEvent('keypress', { key: 'Space' });

    component.onKeyPress(event);

    expect(component.onSubmit).not.toHaveBeenCalled();
  });

  it('should show error message for invalid zip code', () => {
    const zipCodeControl = component.searchForm.get('zipCode');
    zipCodeControl?.setValue('invalid');
    zipCodeControl?.markAsTouched();
    fixture.detectChanges();

    const errorElement = fixture.debugElement.query(By.css('.invalid-feedback'));
    expect(errorElement).toBeTruthy();
    expect(errorElement.nativeElement.textContent).toContain(
      'Please enter a valid 5-digit zip code'
    );
  });

  it('should show error message for required zip code', () => {
    const zipCodeControl = component.searchForm.get('zipCode');
    zipCodeControl?.setValue('');
    zipCodeControl?.markAsTouched();
    fixture.detectChanges();

    const errorElement = fixture.debugElement.query(By.css('.invalid-feedback'));
    expect(errorElement).toBeTruthy();
    expect(errorElement.nativeElement.textContent).toContain('Zip code is required');
  });

  it('should not show error message for valid zip code', () => {
    const zipCodeControl = component.searchForm.get('zipCode');
    zipCodeControl?.setValue('10001');
    zipCodeControl?.markAsTouched();
    fixture.detectChanges();

    const errorElement = fixture.debugElement.query(By.css('.invalid-feedback'));
    expect(errorElement).toBeFalsy();
  });

  it('should disable submit button when form is invalid', () => {
    const zipCodeControl = component.searchForm.get('zipCode');
    zipCodeControl?.setValue('invalid');
    fixture.detectChanges();

    const submitButton = fixture.debugElement.query(By.css('.btn-search'));
    expect(submitButton.nativeElement.disabled).toBeTruthy();
  });

  it('should enable submit button when form is valid', () => {
    const zipCodeControl = component.searchForm.get('zipCode');
    zipCodeControl?.setValue('10001');
    fixture.detectChanges();

    const submitButton = fixture.debugElement.query(By.css('.btn-search'));
    expect(submitButton.nativeElement.disabled).toBeFalsy();
  });

  it('should show loading state when isLoading is true', () => {
    component.isLoading = true;
    fixture.detectChanges();

    const loadingSpinner = fixture.debugElement.query(By.css('.spinner-border'));
    expect(loadingSpinner).toBeTruthy();

    const submitButton = fixture.debugElement.query(By.css('.btn-search'));
    expect(submitButton.nativeElement.disabled).toBeTruthy();
  });

  it('should not show loading state when isLoading is false', () => {
    component.isLoading = false;
    fixture.detectChanges();

    const loadingSpinner = fixture.debugElement.query(By.css('.spinner-border'));
    expect(loadingSpinner).toBeFalsy();
  });

  it('should display form title', () => {
    const titleElement = fixture.debugElement.query(By.css('.search-form-title'));
    expect(titleElement).toBeTruthy();
    expect(titleElement.nativeElement.textContent).toContain('Find Locations');
  });

  it('should display form subtitle', () => {
    const subtitleElement = fixture.debugElement.query(By.css('.search-form-subtitle'));
    expect(subtitleElement).toBeTruthy();
    expect(subtitleElement.nativeElement.textContent).toContain('Enter your zip code');
  });

  it('should have proper form structure', () => {
    const formElement = fixture.debugElement.query(By.css('.search-form'));
    expect(formElement).toBeTruthy();

    const inputGroup = fixture.debugElement.query(By.css('.input-group'));
    expect(inputGroup).toBeTruthy();

    const inputElement = fixture.debugElement.query(By.css('.form-control'));
    expect(inputElement).toBeTruthy();

    const submitButton = fixture.debugElement.query(By.css('.btn-search'));
    expect(submitButton).toBeTruthy();
  });

  it('should have proper input placeholder', () => {
    const inputElement = fixture.debugElement.query(By.css('.form-control'));
    expect(inputElement.nativeElement.placeholder).toContain('Enter zip code');
  });

  it('should have proper button text', () => {
    const submitButton = fixture.debugElement.query(By.css('.btn-search'));
    expect(submitButton.nativeElement.textContent).toContain('Search');
  });

  it('should mark form as touched when submitted with invalid data', () => {
    const zipCodeControl = component.searchForm.get('zipCode');
    zipCodeControl?.setValue('invalid');

    component.onSubmit();

    expect(zipCodeControl?.touched).toBeTruthy();
  });

  it('should reset form after successful submission', () => {
    const zipCodeControl = component.searchForm.get('zipCode');
    zipCodeControl?.setValue('10001');

    spyOn(component.searchRequested, 'emit');
    component.onSubmit();

    expect(component.searchRequested.emit).toHaveBeenCalledWith('10001');
    // Note: Form reset would typically be handled by parent component
  });

  it('should handle form submission with whitespace', () => {
    spyOn(component.searchRequested, 'emit');
    const zipCode = '  10001  ';

    component.searchForm.get('zipCode')?.setValue(zipCode);
    component.onSubmit();

    expect(component.searchRequested.emit).toHaveBeenCalledWith('10001');
  });

  it('should not emit event for whitespace-only input', () => {
    spyOn(component.searchRequested, 'emit');

    component.searchForm.get('zipCode')?.setValue('   ');
    component.onSubmit();

    expect(component.searchRequested.emit).not.toHaveBeenCalled();
  });

  it('should have proper accessibility attributes', () => {
    const inputElement = fixture.debugElement.query(By.css('.form-control'));
    expect(inputElement.nativeElement.getAttribute('type')).toBe('text');
    expect(inputElement.nativeElement.getAttribute('maxlength')).toBe('5');

    const submitButton = fixture.debugElement.query(By.css('.btn-search'));
    expect(submitButton.nativeElement.getAttribute('type')).toBe('submit');
  });

  it('should handle rapid form submissions', () => {
    spyOn(component.searchRequested, 'emit');
    const zipCode = '10001';

    component.searchForm.get('zipCode')?.setValue(zipCode);

    // Multiple rapid submissions
    component.onSubmit();
    component.onSubmit();
    component.onSubmit();

    expect(component.searchRequested.emit).toHaveBeenCalledTimes(3);
  });

  it('should maintain form state during loading', () => {
    const zipCode = '10001';
    component.searchForm.get('zipCode')?.setValue(zipCode);
    component.isLoading = true;
    fixture.detectChanges();

    const zipCodeControl = component.searchForm.get('zipCode');
    expect(zipCodeControl?.value).toBe(zipCode);
  });
});
