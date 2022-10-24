import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommonAttachmentComponent } from './common-attachment.component';

describe('CommonAttachmentComponent', () => {
  let component: CommonAttachmentComponent;
  let fixture: ComponentFixture<CommonAttachmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommonAttachmentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommonAttachmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
