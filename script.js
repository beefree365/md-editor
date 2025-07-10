// Core Editor
import Editor from '@toast-ui/editor';
import '@toast-ui/editor/dist/toastui-editor.css';

// Plugins & Dependencies
import chart from '@toast-ui/editor-plugin-chart';
import codeSyntaxHighlight from '@toast-ui/editor-plugin-code-syntax-highlight/dist/toastui-editor-plugin-code-syntax-highlight-all.js';
import Prism from 'prismjs';
import 'prismjs/themes/prism.css';
import colorSyntax from '@toast-ui/editor-plugin-color-syntax';
import 'tui-color-picker/dist/tui-color-picker.css';
import '@toast-ui/editor-plugin-color-syntax/dist/toastui-editor-plugin-color-syntax.css';
import tableMergedCell from '@toast-ui/editor-plugin-table-merged-cell';
import '@toast-ui/editor-plugin-table-merged-cell/dist/toastui-editor-plugin-table-merged-cell.css';
import uml from '@toast-ui/editor-plugin-uml';
import mermaid from 'mermaid';

// Chart options from the example
const chartOptions = {
  minWidth: 100,
  maxWidth: 600,
  minHeight: 100,
  maxHeight: 300
};

// Fetch content and initialize editor
fetch('example.md')
  .then(response => response.text())
  .then(markdownContent => {
    const editor = new Editor({
      el: document.querySelector('#editor'),
      previewStyle: 'vertical',
      height: '100vh',
      initialValue: markdownContent,
      plugins: [
        [chart, chartOptions],
        [codeSyntaxHighlight, { highlighter: Prism }],
        colorSyntax,
        tableMergedCell,
        // Correctly configure the UML plugin to recognize 'mermaid' language blocks
        [uml, { 
            mermaid,
            languages: ['uml', 'mermaid'] // Explicitly add 'mermaid' as a supported language
        }]
      ]
    });
  });
