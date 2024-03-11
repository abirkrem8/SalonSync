import "../chunks/chunk.NH3SRVOC.js";
import "../chunks/chunk.QPSNFEB2.js";
import {
  registerTranslation
} from "../chunks/chunk.O27EHOBW.js";
import "../chunks/chunk.KIILAQWQ.js";

// src/translations/fa.ts
var translation = {
  $code: "fa",
  $name: "\u0641\u0627\u0631\u0633\u06CC",
  $dir: "rtl",
  carousel: "\u0686\u0631\u062E \u0641\u0644\u06A9",
  clearEntry: "\u067E\u0627\u06A9 \u06A9\u0631\u062F\u0646 \u0648\u0631\u0648\u062F\u06CC",
  close: "\u0628\u0633\u062A\u0646",
  copied: "\u06A9\u067E\u06CC \u0634\u062F",
  copy: "\u0631\u0648\u0646\u0648\u0634\u062A",
  currentValue: "\u0645\u0642\u062F\u0627\u0631 \u0641\u0639\u0644\u06CC",
  error: "\u062E\u0637\u0627",
  goToSlide: (slide, count) => `\u0631\u0641\u062A\u0646 \u0628\u0647 \u0627\u0633\u0644\u0627\u06CC\u062F ${slide} \u0627\u0632 ${count}`,
  hidePassword: "\u067E\u0646\u0647\u0627\u0646 \u06A9\u0631\u062F\u0646 \u0631\u0645\u0632",
  loading: "\u0628\u0627\u0631\u06AF\u0630\u0627\u0631\u06CC",
  nextSlide: "\u0627\u0633\u0644\u0627\u06CC\u062F \u0628\u0639\u062F\u06CC",
  numOptionsSelected: (num) => {
    if (num === 0)
      return "\u0647\u06CC\u0686 \u06AF\u0632\u06CC\u0646\u0647 \u0627\u06CC \u0627\u0646\u062A\u062E\u0627\u0628 \u0646\u0634\u062F\u0647 \u0627\u0633\u062A";
    if (num === 1)
      return "1 \u06AF\u0632\u06CC\u0646\u0647 \u0627\u0646\u062A\u062E\u0627\u0628 \u0634\u062F\u0647 \u0627\u0633\u062A";
    return `${num} \u06AF\u0632\u06CC\u0646\u0647 \u0627\u0646\u062A\u062E\u0627\u0628 \u0634\u062F\u0647 \u0627\u0633\u062A`;
  },
  previousSlide: "\u0627\u0633\u0644\u0627\u06CC\u062F \u0642\u0628\u0644\u06CC",
  progress: "\u067E\u06CC\u0634\u0631\u0641\u062A",
  remove: "\u062D\u0630\u0641",
  resize: "\u062A\u063A\u06CC\u06CC\u0631 \u0627\u0646\u062F\u0627\u0632\u0647",
  scrollToEnd: "\u067E\u06CC\u0645\u0627\u06CC\u0634 \u0628\u0647 \u0627\u0646\u062A\u0647\u0627",
  scrollToStart: "\u067E\u06CC\u0645\u0627\u06CC\u0634 \u0628\u0647 \u0627\u0628\u062A\u062F\u0627",
  selectAColorFromTheScreen: "\u0627\u0646\u062A\u062E\u0627\u0628 \u06CC\u06A9 \u0631\u0646\u06AF \u0627\u0632 \u0635\u0641\u062D\u0647 \u0646\u0645\u0627\u06CC\u0634",
  showPassword: "\u0646\u0645\u0627\u06CC\u0634 \u0631\u0645\u0632",
  slideNum: (slide) => `\u0627\u0633\u0644\u0627\u06CC\u062F ${slide}`,
  toggleColorFormat: "\u062A\u063A\u06CC\u06CC\u0631 \u0642\u0627\u0644\u0628 \u0631\u0646\u06AF"
};
registerTranslation(translation);
var fa_default = translation;
export {
  fa_default as default
};
