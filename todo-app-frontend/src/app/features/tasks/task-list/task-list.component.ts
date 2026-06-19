import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router} from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { TaskService } from '../../../core/services/task.service'; 
import { TaskDto } from '../../../shared/models/task.model'; 

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './task-list.component.html'
})

export class TaskListComponent implements OnInit {
  username: string | null = '';
  tasks: TaskDto[] = [];
  isLoading = false;

  taskForm: FormGroup;
  isSubmitting = false;
  isChecklistMode = false;

  constructor(
    private authService: AuthService, 
    private taskService: TaskService,
    private router: Router,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.taskForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      checklistItems: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.username = localStorage.getItem('todo_user');
    this.loadTasks();
  }

  get checklistItems(): FormArray {
    return this.taskForm.get('checklistItems') as FormArray;
  }

  setMode(isChecklist: boolean): void {
    this.isChecklistMode = isChecklist;
    if (isChecklist && this.checklistItems.length === 0) {
      this.addChecklistItem();
    } else if (!isChecklist) {
      this.checklistItems.clear();
    }
  }

  addChecklistItem(): void {
    const itemGroup = this.fb.group({
      text: ['', Validators.required],
      done: [false]
    });
    this.checklistItems.push(itemGroup);
  }

  removeChecklistItem(index: number): void {
    this.checklistItems.removeAt(index);
  }

  loadTasks(): void {
    this.isLoading = true;
    this.taskService.getTasks().subscribe({
      next: (data) => {
        this.tasks = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error fetching tasks', err);
        this.isLoading = false;
      }
    });
  }

onAddTask(): void {
    if (this.taskForm.invalid) return;

    this.isSubmitting = true;

    const formValue = this.taskForm.value;
    let finalDescription = formValue.description;

    if (this.isChecklistMode) {
      finalDescription = JSON.stringify({
        type: 'checklist',
        items: formValue.checklistItems
      });
    }

    const payload = {
      title: formValue.title,
      description: finalDescription
    };

    this.taskService.createTask(payload).subscribe({
      next: (newTask) => {
        this.tasks.unshift(newTask); 
        this.taskForm.reset();
        this.checklistItems.clear();
        this.isChecklistMode = false;
        this.isSubmitting = false;

        const modalElement = document.getElementById('addTaskModal');
        if (modalElement) {
          const bootstrapModal = (window as any).bootstrap.Modal.getInstance(modalElement);
          if (bootstrapModal) bootstrapModal.hide();
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error creating task', err);
        this.isSubmitting = false;
        this.cdr.detectChanges();
      }
    });
  }

  parseDescription(desc: string): { isChecklist: boolean; text?: string; items?: any[] } {
    try {
      if (desc && desc.startsWith('{"type":"checklist"')) {
        const parsed = JSON.parse(desc);
        return { isChecklist: true, items: parsed.items };
      }
    } catch (e) {}
    return { isChecklist: false, text: desc };
  }

  onLogout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}

