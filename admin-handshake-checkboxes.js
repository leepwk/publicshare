(function addCheckboxListStyles() {
  const style = document.createElement("style");
  style.textContent = `
    .checkbox-list {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
      gap: 10px;
      width: 100%;
    }

    .checkbox-option {
      display: flex;
      align-items: center;
      gap: 10px;
      min-height: 44px;
      padding: 10px 12px;
      border: 1px solid var(--border);
      border-radius: 14px;
      background: #fffdfb;
      color: var(--ink);
      cursor: pointer;
    }

    .checkbox-option input {
      width: auto;
      accent-color: var(--brand);
    }

    @media (max-width: 760px) {
      .checkbox-list {
        grid-template-columns: 1fr;
      }
    }
  `;
  document.head.appendChild(style);
})();
