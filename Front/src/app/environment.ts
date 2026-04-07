const isDockerPort = window.location.port === '4200';
export const environment = {
  production: false,
  apiUrl: isDockerPort ? 'http://localhost:8080/api' : 'https://localhost:44326/api',

  githubID: 'Ov23ct6opUE5dP6aZBsn',
};
