import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CategoryDto, CreateCategoryDto } from '../../shared/models/category.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private readonly API = `${environment.apiUrl}/categories`;

  constructor(private http: HttpClient) {}

  getCategories(): Observable<CategoryDto[]> {
    return this.http.get<CategoryDto[]>(this.API);
  }

  createCategory(dto: CreateCategoryDto): Observable<CategoryDto> {
    return this.http.post<CategoryDto>(this.API, dto);
  }
}