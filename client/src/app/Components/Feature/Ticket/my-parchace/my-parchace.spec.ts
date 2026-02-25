import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyParchace } from './my-parchace';

describe('MyParchace', () => {
  let component: MyParchace;
  let fixture: ComponentFixture<MyParchace>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyParchace]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MyParchace);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
