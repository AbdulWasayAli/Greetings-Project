(() => {
    if (('ontouchstart' in window) || navigator.maxTouchPoints > 0) return;

    const body = document.body;
    let last = 0;
    const interval = 25; // ms between spawns
    const colors = [
        'radial-gradient(circle at 30% 30%, #fff 0%, #ffd700 40%, rgba(255,215,0,0) 80%)',
        'radial-gradient(circle at 40% 40%, #ffe 0%, #ffdd4a 40%, rgba(255,215,0,0) 80%)',
        'radial-gradient(circle at 30% 30%, #ffffff 0%, #ffb6c1 40%, rgba(255,182,193,0) 80%)',
        'radial-gradient(circle at 40% 40%, #e0ffff 0%, #87cefa 40%, rgba(135,206,250,0) 80%)'
    ];

    window.addEventListener('mousemove', (e) => {
        const now = performance.now();
        if (now - last < interval) return;
        last = now;

        for (let i = 0; i < 2; i++) {
            const s = document.createElement('div');
            s.className = 'sparkle';

            const size = 5 + Math.random() * 10;
            const offsetX = (Math.random() - 0.5) * 10;
            const offsetY = (Math.random() - 0.5) * 10;

            s.style.width = `${size}px`;
            s.style.height = `${size}px`;
            s.style.left = `${e.clientX + offsetX}px`;
            s.style.top = `${e.clientY + offsetY}px`;
            s.style.background = colors[Math.floor(Math.random() * colors.length)];
            s.style.animationDuration = `${800 + Math.random() * 500}ms`;

            body.appendChild(s);

            s.addEventListener('animationend', () => s.remove(), { once: true });
        }
    }, { passive: true });
})();
