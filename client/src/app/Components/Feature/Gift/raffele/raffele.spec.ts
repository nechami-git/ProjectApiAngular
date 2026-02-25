import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Raffele } from './raffele';

describe('Raffele', () => {
  let component: Raffele;
  let fixture: ComponentFixture<Raffele>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Raffele]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Raffele);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
