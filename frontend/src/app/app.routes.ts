import { Routes } from '@angular/router';
import { MyReservationsPageComponent } from './features/my-reservations/my-reservations.page';
import { RoomSearchPageComponent } from './features/room-search/room-search.page';

export const appRoutes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'room-search' },
  { path: 'room-search', component: RoomSearchPageComponent },
  { path: 'my-reservations', component: MyReservationsPageComponent },
  { path: '**', redirectTo: 'room-search' }
];
