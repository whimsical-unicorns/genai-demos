import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiSelectionService {
  private apiInternal$: BehaviorSubject<string> = new BehaviorSubject<string>('http://localhost:5259');

  public get api$(): Observable<string> {
    return this.apiInternal$;
  }

  public get api(): string {
    return this.apiInternal$.value;
  }

  public set api(value: string) {
    this.apiInternal$.next(value);
  }
}
