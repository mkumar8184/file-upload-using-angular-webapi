import { TestBed } from '@angular/core/testing';

import { HttpmoduleService } from './httpmodule.service';

describe('HttpmoduleService', () => {
  let service: HttpmoduleService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HttpmoduleService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
