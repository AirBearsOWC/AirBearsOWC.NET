/// <binding AfterBuild="build" Clean="clean" />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    htmlmin = require("gulp-htmlmin"),
    uglify = require("gulp-uglify"),
    rename = require("gulp-rename"),
    templateCache = require("gulp-angular-templatecache"),
    rev = require("gulp-rev"),
    revReplace = require("gulp-rev-replace"),
    sourcemaps = require("gulp-sourcemaps"),
    ngAnnotate = require("gulp-ng-annotate");

var paths = {
    webroot: "./wwwroot/"
};

paths.app = paths.webroot + "app/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "dist/js/*.js";
paths.concatCssDest = paths.webroot + "dist/css/*.css";

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
    paths.webroot + "lib/braintree-web/dist/braintree.js",
    paths.webroot + "lib/lodash/dist/lodash.min.js",
    paths.webroot + "lib/angularjs-toaster/toaster.js",
    paths.webroot + "lib/angular-recaptcha/release/angular-recaptcha.js",
    paths.webroot + "lib/angular-ui-router/release/angular-ui-router.js",
    paths.webroot + "lib/textAngular/dist/textAngular-rangy.min.js",
    paths.webroot + "lib/textAngular/dist/textAngular-sanitize.min.js",
    paths.webroot + "lib/textAngular/dist/textAngular.min.js",
    paths.webroot + "lib/jquery/dist/jquery.js",
    paths.webroot + "lib/bootstrap/dist/js/bootstrap.js"
];

paths.thirdPartyCss = [
    paths.webroot + "lib/angular-loading-bar/build/loading-bar.css",
    paths.webroot + "lib/textAngular/dist/textAngular.css",
    paths.webroot + "lib/font-awesome/css/font-awesome.min.css",
    paths.webroot + "lib/angular-google-places-autocomplete/dist/autocomplete.min.css",
    paths.webroot + "lib/angularjs-toaster/toaster.css",
    paths.webroot + "lib/bootstrap/dist/css/bootstrap.css"
];

gulp.task("clean", function (cb) {
    rimraf(paths.webroot + "dist", cb);
});

gulp.task("min:js", ["clean"], function () {
    var scriptPaths = paths.thirdPartyJs;
    scriptPaths.push(paths.webroot + "app/app.js");
    scriptPaths.push(paths.app);

    return gulp.src(scriptPaths, { base: "." })
        .pipe(ngAnnotate())
        .pipe(sourcemaps.init())
        .pipe(concat("combined.js"))
        .pipe(uglify())
        .pipe(sourcemaps.write())
        .pipe(gulp.dest("./wwwroot/dist/js"));
});

gulp.task("min:css", ["clean"], function () {
    var stylePaths = paths.thirdPartyCss;
    stylePaths.push(paths.css);

    return gulp.src(stylePaths, { base: "." })
        .pipe(concat("combined.css"))
        .pipe(cssmin())
        .pipe(gulp.dest("./wwwroot/dist/css"));
});

gulp.task("move:fonts", ["clean"], function () {
    var fontPaths = [paths.webroot + "lib/bootstrap/dist/fonts/*", paths.webroot + "lib/font-awesome/fonts/*"];

    return gulp.src(fontPaths)
        .pipe(gulp.dest(paths.webroot + "dist/fonts"));
});

gulp.task("move:downloads", ["clean"], function () {
    var downloadPaths = [paths.webroot + "downloads/*"];

    return gulp.src(downloadPaths)
        .pipe(gulp.dest(paths.webroot + "dist/downloads"));
});

gulp.task("templatecache", ["clean"], function () {
    return gulp.src(paths.webroot + "app/**/*.html")
        .pipe(htmlmin())
        .pipe(templateCache({ root: "app/" }))
        .pipe(gulp.dest("./wwwroot/dist/js"));
});

gulp.task("rev-assets", ["min:js", "min:css", "templatecache"], function () {
    return gulp.src([paths.concatJsDest, paths.concatCssDest], { base: "./wwwroot/dist" })
       .pipe(rev())
       .pipe(gulp.dest("./wwwroot/dist"))  // write rev'd assets to build dir
       .pipe(rev.manifest())
       .pipe(gulp.dest("./wwwroot/dist")); // write manifest to build dir
});

gulp.task("revreplace", ["rev-assets"], function () {
    var manifest = gulp.src(paths.webroot + "dist/rev-manifest.json");

    return gulp.src(paths.webroot + "/src.html")
      .pipe(revReplace({ manifest: manifest }))
      .pipe(rename("index.html"))
      .pipe(gulp.dest(paths.webroot));
});

gulp.task("build", ["min:js", "min:css", "move:fonts", "move:downloads", "templatecache", "rev-assets", "revreplace"]);
