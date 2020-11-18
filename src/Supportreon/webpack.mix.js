const mix = require('laravel-mix')

mix.js('resources/js/app.js', 'wwwroot/js')
    .sass('resources/sass/app.scss', 'wwwroot/css')
    .setPublicPath('wwwroot')
    .version()