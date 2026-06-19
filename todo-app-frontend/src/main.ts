import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';

console.time('Angular startup');

bootstrapApplication(App, appConfig)
  .then(() => console.timeEnd('Angular startup'))
  .catch(err => console.error(err));