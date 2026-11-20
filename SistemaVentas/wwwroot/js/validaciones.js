(() => {
  const onlyLetters = (value) => value.replace(/[^A-Za-zÁÉÍÓÚÜÑáéíóúüñ\s]/g, '');
  const onlyPhone = (value) => value.replace(/[^\d+]/g, '').replace(/(?!^)\+/g, '');
  const onlyInteger = (value) => value.replace(/[^\d]/g, '');
  const onlyDecimal = (value) => {
    let cleaned = value.replace(/[^\d.]/g, '');
    const parts = cleaned.split('.');
    if (parts.length > 2) cleaned = parts[0] + '.' + parts.slice(1).join('');
    if (parts[1]?.length > 2) cleaned = parts[0] + '.' + parts[1].slice(0, 2);
    return cleaned;
  };

  const bindFilter = (selector, filterFn) => {
    document.querySelectorAll(selector).forEach((el) => {
      el.addEventListener('input', () => {
        const next = filterFn(el.value);
        if (el.value !== next) el.value = next;
      });
      el.addEventListener('paste', (e) => {
        e.preventDefault();
        const text = (e.clipboardData || window.clipboardData).getData('text');
        el.value = filterFn(text);
        el.dispatchEvent(new Event('input', { bubbles: true }));
      });
    });
  };

  bindFilter('[data-val-letters]', onlyLetters);
  bindFilter('[data-val-phone]', onlyPhone);
  bindFilter('[data-val-integer]', onlyInteger);
  bindFilter('[data-val-decimal]', onlyDecimal);
})();
