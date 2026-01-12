const isLocal = window.location.hostname === 'localhost';

export const environment = {
  production: !isLocal,
  apiUrl: isLocal 
    ? 'https://localhost:44326/api' 
    : 'https://invento-api.onrender.com/api' // رابط ريندر بتاعك هنا
};