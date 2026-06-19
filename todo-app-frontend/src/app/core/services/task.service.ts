import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateTaskDto, TaskDto, UpdateTaskDto } from '../../shared/models/task.model';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private readonly API = `${environment.apiUrl}/tasks`;

  constructor(private http: HttpClient) {}

  getTasks(): Observable<TaskDto[]> {
    return this.http.get<TaskDto[]>(this.API);
  }

  createTask(dto: CreateTaskDto): Observable<TaskDto> {
    return this.http.post<TaskDto>(this.API, dto);
  }

  updateTask(id: number, dto:UpdateTaskDto): Observable<TaskDto> {
    return this.http.put<TaskDto>(`${this.API}/${id}`, dto);
  }
}