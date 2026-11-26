/*
 * PR1ME Jelly Theme: tiny page marker.
 * The theme is CSS. This script only tags <html> with the current page type
 * (data-pr1me-page) so styles can stay out of the way, e.g. on the video page.
 * It does not patch Jellyfin, observe the DOM, or touch playback.
 */
(function () {
  'use strict';

  try {
    if (window.__pr1meJellyTheme) {
      return;
    }
    window.__pr1meJellyTheme = true;

    if (window.localStorage && window.localStorage.getItem('pr1me-off') === '1') {
      return;
    }

    var root = document.documentElement;

    function pageType() {
      var hash = (window.location.hash || '').toLowerCase();
      if (hash.indexOf('#/video') === 0) { return 'video'; }
      if (hash.indexOf('#/details') === 0) { return 'details'; }
      if (hash.indexOf('#/login') === 0) { return 'login'; }
      if (hash.indexOf('#/dashboard') === 0) { return 'dashboard'; }
      if (hash.indexOf('#/home') === 0 || hash === '' || hash === '#/') { return 'home'; }
      return 'other';
    }

    function update() {
      try {
        root.setAttribute('data-pr1me-page', pageType());
      } catch (e) { /* ignore */ }
    }

    update();
    window.addEventListener('hashchange', update, { passive: true });
    document.addEventListener('viewshow', update, { passive: true });
    root.setAttribute('data-pr1me', 'on');
  } catch (e) {
    /* never let the theme break the app */
  }
})();
