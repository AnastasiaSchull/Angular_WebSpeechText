const { env } = require('process');

//const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
//    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7229';

const PROXY_CONFIG = [
  {
    context: [
      "/weatherforecast",
      "/api"
    ],
    target: "http://localhost:7229",
    secure: false,
    changeOrigin: true, // Иногда это нужно для обхода проблем с CORS
    logLevel: "debug" // Поможет в отладке, показывая в консоли все проксируемые запросы
  }
]

module.exports = PROXY_CONFIG;
