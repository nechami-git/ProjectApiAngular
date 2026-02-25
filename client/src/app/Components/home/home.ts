import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { ToolbarModule } from 'primeng/toolbar';

@Component({
  selector: 'app-home',
  imports: [ToolbarModule, ButtonModule, RouterModule],
  templateUrl: './home.html',
  styleUrls: ['./home.scss'],
})
export class Home {

}
