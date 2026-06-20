import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { TaskService } from '../../../core/services/task.service'; 
import { CategoryService } from '../../../core/services/category.service';
import { TaskDto } from '../../../shared/models/task.model'; 
import { CategoryDto } from '../../../shared/models/category.model';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './task-list.component.html'
})
export class  TaskListComponent implements OnInit {
  // User and list data
  username: string | null = '';
  tasks: TaskDto[] = [];
  
  // Loading and submitting states
  isLoading = false;
  isSubmitting = false;

  // Modal window (Form) states
  taskForm: FormGroup;
  isChecklistMode = false;
  isEditMode = false;
  editingTaskId: number | null = null;

  // User and list data
  categories: CategoryDto[] = [];
  selectedCategoryId: number | null = null; 
  isAddingCategory = false;

  // Pagination
  currentPage = 1;
  pageSize = 4;
  totalPages = 1;
  totalItems = 0;

  constructor(
    private authService: AuthService, 
    private taskService: TaskService,
    private categoryService: CategoryService,
    private router: Router,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.taskForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      dueDate: [''],
      categoryId: [null],
      checklistItems: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.username = localStorage.getItem('todo_user');
    this.loadTasks();
    this.loadCategories();
  }

  // ==========================================
  // GETTERS AND FORM SERVICE METHODS
  // ==========================================

  get checklistItems(): FormArray {
    return this.taskForm.get('checklistItems') as FormArray;
  }

  get filteredTasks(): TaskDto[] {
    if (this.selectedCategoryId === null) {
      return this.tasks;
    }
    return this.tasks.filter(t => t.categoryId === this.selectedCategoryId);
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

  parseDescription(desc: string): { isChecklist: boolean; text?: string; items?: any[] } {
    try {
      if (desc && desc.startsWith('{"type":"checklist"')) {
        const parsed = JSON.parse(desc);
        return { isChecklist: true, items: parsed.items };
      }
    } catch (e) {}
    return { isChecklist: false, text: desc };
  }

  // ==========================================
  // BACKEND INTERACTION LOGIC (CRUD)
  // ==========================================

  loadTasks(): void {
    this.isLoading = true;
    this.taskService.getTasks(this.currentPage, this.pageSize, this.selectedCategoryId).subscribe({
      next: (data) => {
        this.tasks = data.items || [];
        this.currentPage = data.pageNumber;
        this.totalPages = data.totalPages;
        this.totalItems = data.totalItems;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error fetching tasks', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadCategories(): void {
  this.categoryService.getCategories().subscribe({
    next: (data) => {
      this.categories = data || [];
      this.cdr.detectChanges();
    },
    error: (err) => console.error('Error fetching categories', err)
  });
}

  openCreateModal(): void {
    this.isEditMode = false;
    this.editingTaskId = null;
    this.isChecklistMode = false;
    this.taskForm.reset({ title: '', description: '', dueDate: '', categoryId: null });
    this.checklistItems.clear();
    this.openModalWindow();
  }

  onEditTask(task: TaskDto): void {
    this.isEditMode = true;
    this.editingTaskId = task.id;
    this.checklistItems.clear();

    const info = this.parseDescription(task.description);
    const formattedDate = task.dueDate ? task.dueDate.substring(0, 10) : '';

    if (info.isChecklist) {
      this.isChecklistMode = true;
      info.items?.forEach(item => {
        this.checklistItems.push(this.fb.group({
          text: [item.text, Validators.required],
          done: [item.done]
        }));
      });
      this.taskForm.patchValue({ 
        title: task.title, 
        description: '', 
        dueDate: formattedDate, 
        categoryId: task.categoryId });
    } else {
      this.isChecklistMode = false;
      this.taskForm.patchValue({ 
        title: task.title, 
        description: info.text, 
        dueDate: formattedDate, 
        categoryId: task.categoryId });
    }

    this.openModalWindow();
  }

  onSaveTask(): void {
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

    const rawCategory = formValue.categoryId;
    const parsedCategoryId = (rawCategory === 'null' || rawCategory === '' || rawCategory === null) 
      ? null 
      : rawCategory;

    const parsedDueDate = formValue.dueDate && formValue.dueDate.trim() !== '' 
    ? formValue.dueDate 
    : undefined;

    if (this.isEditMode && this.editingTaskId !== null) {
      // CASE: EDITING AN EXISTING TASK
      const currentTask = this.tasks.find(t => t.id === this.editingTaskId);
      const payload = {
        title: formValue.title,
        description: finalDescription ? finalDescription : '',
        isCompleted: currentTask ? currentTask.isCompleted : false,
        dueDate: parsedDueDate,
        categoryId: parsedCategoryId
      };

      this.taskService.updateTask(this.editingTaskId, payload).subscribe({
        next: () => {
          this.closeAndResetModal();
          this.loadTasks();
        },
        error: (err) => {
          console.error('Error updating task', err);
          this.isSubmitting = false;
          this.cdr.detectChanges();
        }
      });
    } else {
      // CASE: CREATING A NEW TASK
      const payload = { 
        title: formValue.title, 
        description: finalDescription ? finalDescription : '',
        dueDate: parsedDueDate,
        categoryId: parsedCategoryId 
      };

      this.taskService.createTask(payload).subscribe({
        next: () => {
          this.currentPage = 1;
          this.closeAndResetModal();
          this.loadTasks(); // Загружаем свежие данные
        },
        error: (err) => {
          console.error('Error creating task', err);
          this.isSubmitting = false;
          this.cdr.detectChanges();
        }
      });
    }
  }

  toggleChecklistItem(task: TaskDto, itemIndex: number): void {
    const info = this.parseDescription(task.description);
    
    if (info.isChecklist && info.items) {
      info.items[itemIndex].done = !info.items[itemIndex].done;

      const updatedDescription = JSON.stringify({
        type: 'checklist',
        items: info.items
      });

      const updatedTaskPayload = {
        title: task.title,
        description: updatedDescription,
        isCompleted: task.isCompleted,
        dueDate: task.dueDate,      
        categoryId: task.categoryId   
      };

      const index = this.tasks.findIndex(t => t.id === task.id);
      if (index !== -1) {
        this.tasks[index].description = updatedDescription;
        this.cdr.detectChanges();
      }

      this.taskService.updateTask(task.id, updatedTaskPayload).subscribe({
        next: (updatedTask) => {
          if (index !== -1) {
            this.tasks[index] = updatedTask;
            this.cdr.detectChanges();
          }
        },
        error: (err) => {
          console.error('Error updating checklist item', err);
          this.loadTasks();
        }
      });
    }
  }

  toggleTaskCompletion(task: TaskDto): void {
    const newCompletionStatus = !task.isCompleted;

    const payload = {
      title: task.title,
      description: task.description,
      isCompleted: newCompletionStatus,
      dueDate: task.dueDate,
      categoryId: task.categoryId
    };

    const index = this.tasks.findIndex(t => t.id === task.id);
    if (index !== -1) {
      this.tasks[index].isCompleted = newCompletionStatus;
      this.cdr.detectChanges();
    }

    this.taskService.updateTask(task.id, payload).subscribe({
      next: (updatedTask) => {
        if (index !== -1) {
          this.tasks[index] = updatedTask;
          this.cdr.detectChanges();
        }
      },
      error: (err) => {
        console.error('Error toggling task completion', err);
        this.loadTasks();
      }
    });
  }

  onDeleteTask(id: number): void {
    if (confirm('Are you sure you want to delete this task?')) {
      const originalTasks = [...this.tasks];
      this.tasks = this.tasks.filter(t => t.id !== id);
      this.cdr.detectChanges();

      this.taskService.deleteTask(id).subscribe({
        next: () => {
          console.log(`Task ${id} deleted successfully`);
          this.loadTasks();
        },
        error: (err) => {
          console.error('Error deleting task', err);
          this.tasks = originalTasks;
          this.cdr.detectChanges();
          alert('Failed to delete the task. Rolled back.');
        }
      });
    }
}

  onAddCategoryDirect(name: string): void {
    if (!name || name.trim().length < 2) return;

    this.categoryService.createCategory({ name: name.trim() }).subscribe({
      next: (newCat) => {
        this.categories.push(newCat);
        this.isAddingCategory = false;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error creating category', err)
    });
  }

  selectCategory(categoryId: number | null): void {
    this.selectedCategoryId = categoryId;
    this.currentPage = 1;
    this.loadTasks();
  }

  onPageChange(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.loadTasks();
  }

  // ==========================================
  // INTERFACE HELPERS (UI METHODS)
  // ==========================================

  private openModalWindow(): void {
    const modalElement = document.getElementById('addTaskModal');
    if (modalElement) {
      const bootstrapModal = (window as any).bootstrap.Modal.getOrCreateInstance(modalElement);
      bootstrapModal.show();
    }
  }

  private closeAndResetModal(): void {
    this.taskForm.reset();
    this.checklistItems.clear();
    this.isSubmitting = false;
    this.isEditMode = false;
    this.editingTaskId = null;

    const modalElement = document.getElementById('addTaskModal');
    if (modalElement) {
      const bootstrapModal = (window as any).bootstrap.Modal.getInstance(modalElement);
      if (bootstrapModal) bootstrapModal.hide();
    }
    this.cdr.detectChanges();
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}