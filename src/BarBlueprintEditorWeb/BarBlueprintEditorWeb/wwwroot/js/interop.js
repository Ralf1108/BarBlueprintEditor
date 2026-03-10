window.getBoundingClientRect = (element) => {
    if (!element) return { width: 512, height: 512 };
    const rect = element.getBoundingClientRect();
    return { width: rect.width, height: rect.height };
};