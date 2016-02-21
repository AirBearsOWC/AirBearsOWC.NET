/// <binding AfterBuild='min' Clean='clean' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    sourcemaps = require("gulp-sourcemaps");

var paths = {
    webroot: "./wwwroot/"
};

paths.app = paths.webroot + "app/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/combined.js";
paths.concatCssDest = paths.webroot + "css/combined.css";

paths.thirdPartyJs = [
    paths.webroot + "lib/angular/angular.js",
    paths.webroot + "lib/a0-angular-storage/dist/angular-storage.js",
    paths.webroot + "lib/angular-animate/angular-animate.js",
    paths.webroot + "lib/angular-bootstrap/ui-bootstrap.js",
    paths.webroot + "lib/angular-bootstrap/ui-bootstrap-tpls.js",
    paths.webroot + "lib/angular-loading-bar/build/loading-bar.js",
    paths.webroot + "lib/angular-google-places-autocomplete/dist/autocomplete.min.js",
    paths.webroot + "lib/angular-google-maps/dist/angular-google-maps.min.js",
    paths.webroot + "lib/angular-simple-logger/dist/angular-simple-logger.min.js",
    paths.webroot + "lib/lodash/dist/lodash.min.js",
    paths.webroot + "lib/angularjs-toaster/toaster.js",
    paths.webroot + "lib/angular-ui-router/release/angular-ui-router.js",
    paths.webroot + "lib/jquery/dist/jquery.js",
    paths.webroot + "lib/bootstrap/dist/js/bootstrap.js"
];

paths.thirdPartyCss = [
    paths.webroot + "lib/angular-loading-bar/build/loading-bar.css",
    paths.webroot + "lib/angular-google-places-autocomplete/dist/autocomplete.min.css",
    paths.webroot + "lib/angularjs-toaster/toaster.css",
    paths.webroot + "lib/bootstrap/dist/css/bootstrap.css"
];

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function () {
    var scriptPaths = paths.thirdPartyJs;
    scriptPaths.push(paths.webroot + "app/app.js");
    scriptPaths.push(paths.app);
    scriptPaths.push("!" + paths.concatJsDest);

    return gulp.src(scriptPaths, { base: "." })
        .pipe(sourcemaps.init())
        .pipe(concat("combined.js"))
        .pipe(uglify())
        .pipe(sourcemaps.write())
        .pipe(gulp.dest("./wwwroot/js"));
});

gulp.task("min:css", function () {
    var stylePaths = paths.thirdPartyCss;
    stylePaths.push(paths.css);
    stylePaths.push("!" + paths.concatCssDest);

    return gulp.src(stylePaths)
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("move:fonts", function () {
    var fontPaths = paths.webroot + "lib/bootstrap/dist/fonts/*";

    return gulp.src(fontPaths)
        .pipe(gulp.dest(paths.webroot + "fonts"));
});

gulp.task("min", ["min:js", "min:css", "move:fonts"]);
