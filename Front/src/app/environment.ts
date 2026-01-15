const isLocal = window.location.hostname === 'localhost';

export const environment = {
  production: !isLocal,
  apiUrl: isLocal 
    ? 'https://localhost:44326/api' 
    : 'https://inventoserver.up.railway.app/api'
  };