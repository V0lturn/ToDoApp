import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateTaskDto, PagedResponse, TaskDto, UpdateTaskDto } from '../../shared/models/task.model';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private readonly API = `${environment.apiUrl}/tasks`;

  constructor(private http: HttpClient) {}

  getTasks(page: number, pageSize: number, categoryId: number | null): Observable<PagedResponse<TaskDto>> {
    let params = new HttpParams()
      .set('pageNumber', page.toString())
      .set('pageSize', pageSize.toString());

    if (categoryId !== null) {
      params = params.set('categoryId', categoryId.toString());
    }

    return this.http.get<PagedResponse<TaskDto>>(this.API, { params });
  }

  createTask(dto: CreateTaskDto): Observable<TaskDto> {
    return this.http.post<TaskDto>(this.API, dto);
  }

  updateTask(id: number, dto:UpdateTaskDto): Observable<TaskDto> {
    return this.http.put<TaskDto>(`${this.API}/${id}`, dto);
  }

  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API}/${id}`);
  }
}