/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          500: '#3b82f6', // تقدر تغير اللون ده حسب براند Invento
        }
      }
    },
  },
  plugins: [],
}