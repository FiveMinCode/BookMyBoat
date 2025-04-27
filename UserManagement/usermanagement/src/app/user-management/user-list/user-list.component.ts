import { Component, OnInit } from '@angular/core';
import { UserService } from '../user.service';
import { User } from '../user';
import { BehaviorSubject, catchError, debounceTime, filter, forkJoin, from, fromEvent, map, merge, Observable, of, ReplaySubject, share, Subject, Subscriber, switchMap, tap, throttleTime, throwError } from 'rxjs';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-user-list',
  imports: [CommonModule, RouterModule],
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss'],
})
export class UserListComponent implements OnInit {
  users$: Observable<User[]> | undefined;
  
  constructor(private userService: UserService, private router: Router) {}
  viewDetails(id: number): void {
    this.router.navigate(['/user', id]);
  }
  
  ngOnInit(): void {
    this.users$ = this.userService.getUsers();

    //Cold Observable
    const userList$: Observable<string[]> = new Observable(subscriber => {
      subscriber.next(['User1', 'User2', 'User3']);
      subscriber.complete();
    })
    userList$.subscribe((users: string[]) => console.log(users));

    // Hot Observable
    const userSelected$: Subject<string> = new Subject();
    userSelected$.subscribe((user: string) => console.log(`Selected: ${user}`));
    userSelected$.next('User1');

    //Merge
    const userList1$: Observable<string[]> = of(['User1', 'User2']);
    const userDetails$: Observable<{ id: number; name: string; details: string }> = of({
      id: 1,
      name: 'User1',
      details: 'Some details',
    });
    merge(userList1$, userDetails$).subscribe((data: any) => console.log(data)); // Output: User List and Details will appear sequentially

    // ForJoin
    const userList2$: Observable<string[]> = of(['User1', 'User2']);
    const userDetails2$: Observable<{ id: number; name: string; details: string }> = of({
      id: 1,
      name: 'User1',
      details: 'Some details',
    });
    forkJoin({ userList2$, userDetails2$ }).subscribe(results => console.log(results));

    //Map: Transform user names to uppercase.
    of(['User1', 'User2']).pipe(
      tap((users: string[]) =>  console.log(users)),
      map((users: string[]) => users.map(user => user.toUpperCase()))
    ).subscribe((users: string[]) => console.log(users)); 

    //SwitchMap: Fetch user details when a user is selected.
    const userSelected3$: Observable<string> = of('User1');
    userSelected3$.pipe(
      switchMap((user: string) =>
        of({ id: 1, name: user, details: 'Details here' })
      )
    ).subscribe(details => console.log(details)); 

    //Filter: Emit users whose names start with "U".
    from(['User1', 'Admin', 'User2']).pipe(
      filter((user: string) => user.startsWith('U'))
    ).subscribe((user: string) => console.log(user));

    //DebounceTime: Handle user typing in a search box.
    const searchBox = document.querySelector('#searchBox') as HTMLInputElement;
    fromEvent<InputEvent>(searchBox, 'input').pipe(
      debounceTime(300), // Emit after 300ms of inactivity
      map(event => (event.target as HTMLInputElement).value)
    ).subscribe(value => console.log(value));

    //CatchError: Provide fallback data when an error occurs.
    const userDetails4$: Observable<any> = throwError('Error fetching user details');
    userDetails4$.pipe(
      catchError(error => {
        console.error(error);
        return of({ error: 'Fallback user details' });
      })
    ).subscribe(details => console.log(details));

    //ThrottleTime: Throttle frequent user actions.
    const userSelect$ = fromEvent<MouseEvent>(document, 'click'); // Simulating user selection
    userSelect$.pipe(
      throttleTime(2000) // Emit clicks every 2 seconds
    ).subscribe(() => console.log('User selected!'));

    //ReplaySubject: Allows subscribers to receive previously emitted values even after subscription.
    const userReplay$ = new ReplaySubject<string>(2); // Buffer size is 2
    userReplay$.next('User1');
    userReplay$.next('User2');
    userReplay$.next('User3');
    userReplay$.subscribe(user => console.log(`Replayed User: ${user}`));

    //BehaviorSubject: Emits the latest value to new subscribers.
    const currentUser$ = new BehaviorSubject<string>('User1'); // Initial value
    currentUser$.subscribe(user => console.log(`Current User: ${user}`));
    currentUser$.next('User2'); 

    //Share: Share a single subscription across multiple subscribers, converting a cold observable into a hot observable.
    const userList5$ = of(['User1', 'User2', 'User3']).pipe(share());
    userList5$.subscribe(users => console.log('Subscriber 1:', users));
    userList5$.subscribe(users => console.log('Subscriber 2:', users));
  }

}