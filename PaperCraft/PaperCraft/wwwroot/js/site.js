// Initialize AOS with enhanced settings
AOS.init({
    duration: 1000,
    once: true,
    easing: 'ease-out-cubic'
});

// Global variables
let products = [];
let cart = [];
let currentPage = 'home';

// Load products data from API
async function loadProductsData() {
    try {
        const resp = await fetch('/api/products');
        if (!resp.ok) throw new Error(`Status ${resp.status}`);
        products = await resp.json();
    } catch (err) {
        console.error('Не удалось загрузить товары:', err);
    }
}

// Load and display products
function loadProducts() {
    const productList = document.getElementById('product-list');
    if (!productList) return;

    productList.innerHTML = '';
    products.forEach((product, index) => {
        const card = document.createElement('div');
        card.className = 'col-lg-3 col-md-4 col-sm-6 mb-4';
        card.setAttribute('data-aos', 'fade-up');
        card.setAttribute('data-aos-delay', (index % 4) * 100);
        card.setAttribute('data-product-id', product.id);
        card.innerHTML = `
            <div class="product-card card h-100">
                <img src="${product.imageUrl}" class="card-img-top" alt="${product.name}">
                <div class="card-body d-flex flex-column">
                    <h5 class="card-title">${product.name}</h5>
                    <p class="card-text text-muted">${product.description}</p>
                    <div class="mt-auto">
                        <div class="product-price mb-3">${product.price} ₽</div>
                        <button class="btn btn-add-cart w-100" onclick="addToCart(${product.id})">
                            <i class="fas fa-cart-plus me-2"></i>В корзину
                        </button>
                    </div>
                </div>
            </div>
        `;
        productList.appendChild(card);
    });
    AOS.refresh();
}

// Load featured products for home page
function loadFeaturedProducts() {
    const featuredList = document.getElementById('featured-product-list');
    if (!featuredList) return;

    featuredList.innerHTML = '';
    products.slice(0, 4).forEach((product, index) => {
        const card = document.createElement('div');
        card.className = 'col-lg-3 col-md-4 col-sm-6 mb-4';
        card.setAttribute('data-aos', 'fade-up');
        card.setAttribute('data-aos-delay', index * 100);
        card.innerHTML = `
            <div class="product-card card h-100">
                <img src="${product.imageUrl}" class="card-img-top" alt="${product.name}">
                <div class="card-body d-flex flex-column">
                    <h5 class="card-title">${product.name}</h5>
                    <p class="card-text text-muted">${product.description}</p>
                    <div class="mt-auto">
                        <div class="product-price mb-3">${product.price} ₽</div>
                        <button class="btn btn-add-cart w-100" onclick="addToCart(${product.id})">
                            <i class="fas fa-cart-plus me-2"></i>В корзину
                        </button>
                    </div>
                </div>
            </div>
        `;
        featuredList.appendChild(card);
    });
}

// Cart management functions
function addToCart(productId) {
    const product = products.find(p => p.id === productId);
    if (!product) return;

    const existing = cart.find(item => item.id === productId);
    if (existing) {
        existing.quantity++;
    } else {
        cart.push({ ...product, quantity: 1 });
    }
    updateCartUI();
    showToast('Товар добавлен в корзину!', 'success');
}

function removeFromCart(productId) {
    cart = cart.filter(item => item.id !== productId);
    updateCartUI();
}

function updateQuantity(productId, delta) {
    const item = cart.find(i => i.id === productId);
    if (!item) return;

    item.quantity += delta;
    if (item.quantity <= 0) {
        removeFromCart(productId);
    } else {
        updateCartUI();
    }
}

// Update cart UI
function updateCartUI() {
    const cartCount = document.getElementById('cart-count');
    const cartItems = document.getElementById('cart-items');
    const cartTotal = document.getElementById('cart-total');

    if (!cartCount || !cartItems || !cartTotal) return;

    const totalItems = cart.reduce((sum, i) => sum + i.quantity, 0);
    const totalPrice = cart.reduce((sum, i) => sum + i.quantity * i.price, 0);

    cartCount.textContent = totalItems;
    cartTotal.textContent = totalPrice.toLocaleString() + ' ₽';

    if (cart.length === 0) {
        cartItems.innerHTML = `
            <div class="text-center py-5">
                <i class="fas fa-shopping-cart" style="font-size: 4rem; color: #ddd"></i>
                <p class="mt-3">Корзина пуста</p>
            </div>
        `;
    } else {
        cartItems.innerHTML = cart.map(item => `
            <div class="cart-item d-flex justify-content-between align-items-center mb-3 p-3 border rounded">
                <div class="d-flex align-items-center">
                    <img src="${item.imageUrl}" alt="${item.name}" 
                         style="width:60px;height:60px;object-fit:cover;border-radius:8px" class="me-3">
                    <div>
                        <h6 class="mb-0">${item.name}</h6>
                        <small class="text-muted">${item.price.toLocaleString()} ₽</small>
                    </div>
                </div>
                <div class="d-flex align-items-center">
                    <div class="quantity-controls me-3">
                        <button class="btn btn-sm btn-outline-secondary quantity-btn" onclick="updateQuantity(${item.id}, -1)">
                            <i class="fas fa-minus"></i>
                        </button>
                        <span class="mx-2 fw-bold">${item.quantity}</span>
                        <button class="btn btn-sm btn-outline-secondary quantity-btn" onclick="updateQuantity(${item.id}, 1)">
                            <i class="fas fa-plus"></i>
                        </button>
                    </div>
                    <button class="btn btn-sm btn-outline-danger" onclick="removeFromCart(${item.id})">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </div>
        `).join('');
    }
}

// Enhanced toast function
function showToast(message, type = 'success') {
    // Try to use toast container if it exists
    let toastContainer = document.querySelector('.toast-container');

    if (toastContainer) {
        const toast = document.createElement('div');
        toast.className = `custom-toast ${type} p-3 mb-2`;

        const icon = type === 'success' ? 'fas fa-check-circle' : 'fas fa-exclamation-circle';
        const color = type === 'success' ? 'var(--success-color, #28a745)' : 'var(--danger-color, #dc3545)';

        toast.innerHTML = `
            <div class="d-flex align-items-center">
                <i class="${icon} me-2" style="color: ${color}; font-size: 1.2rem;"></i>
                <span>${message}</span>
                <button type="button" class="btn-close ms-auto" onclick="this.parentElement.parentElement.remove()"></button>
            </div>
        `;

        toastContainer.appendChild(toast);

        // Auto remove after 5 seconds
        setTimeout(() => {
            if (toast.parentElement) {
                toast.style.animation = 'slideInRight 0.3s ease-out reverse';
                setTimeout(() => toast.remove(), 300);
            }
        }, 5000);
    } else {
        // Fallback to original toast implementation
        const toast = document.createElement('div');
        toast.className = `alert alert-${type} position-fixed`;
        toast.style.cssText = 'top:20px;right:20px;z-index:9999;min-width:300px;';
        toast.innerHTML = `${message}<button type="button" class="btn-close" onclick="this.parentElement.remove()"></button>`;
        document.body.appendChild(toast);

        setTimeout(() => {
            if (toast.parentElement) toast.remove();
        }, 3000);
    }
}

// Page navigation
function showPage(page) {
    document.querySelectorAll('.page-content').forEach(p => p.classList.add('d-none'));
    const targetPage = document.getElementById(`${page}-page`);
    if (targetPage) {
        targetPage.classList.remove('d-none');
    }

    currentPage = page;

    // Update navigation active state
    document.querySelectorAll('.nav-link').forEach(link => link.classList.remove('active'));
    const activeLink = document.querySelector(`.nav-link[href="#${page}"]`);
    if (activeLink) activeLink.classList.add('active');

    // Page-specific actions
    if (page === 'products') {
        resetFilters();
        loadProducts();
        filterProducts();
    }
    if (page === 'profile') {
        loadUserProfile();
    }

    window.scrollTo(0, 0);

    // Refresh AOS animations for new page
    setTimeout(() => {
        AOS.refresh();
    }, 100);
}

// Filter and search functions
function resetFilters() {
    const searchInput = document.getElementById('search-input');
    const categorySelect = document.getElementById('category-select');
    const sortSelect = document.getElementById('sort-select');

    if (searchInput) searchInput.value = '';
    if (categorySelect) categorySelect.value = '';
    if (sortSelect) sortSelect.value = '';
}

function filterProducts() {
    const searchInput = document.getElementById('search-input');
    const categorySelect = document.getElementById('category-select');

    if (!searchInput || !categorySelect) return;

    const term = searchInput.value.toLowerCase();
    const selCat = categorySelect.value;

    document.querySelectorAll('#product-list .col-lg-3').forEach(card => {
        const id = parseInt(card.getAttribute('data-product-id'));
        const prod = products.find(p => p.id === id);

        if (prod) {
            const matchesSearch = (prod.name + prod.description).toLowerCase().includes(term);
            const matchesCategory = !selCat || prod.category === selCat;
            card.style.display = matchesSearch && matchesCategory ? 'block' : 'none';
        }
    });
}

function sortProducts() {
    const sortSelect = document.getElementById('sort-select');
    const container = document.getElementById('product-list');

    if (!sortSelect || !container) return;

    const sortBy = sortSelect.value;
    const cards = Array.from(container.children);

    if (!sortBy) return;

    cards.sort((a, b) => {
        const pa = products.find(p => p.id === parseInt(a.getAttribute('data-product-id')));
        const pb = products.find(p => p.id === parseInt(b.getAttribute('data-product-id')));

        if (!pa || !pb) return 0;

        switch (sortBy) {
            case 'price-low': return pa.price - pb.price;
            case 'price-high': return pb.price - pa.price;
            case 'name': return pa.name.localeCompare(pb.name);
            case 'category': return pa.category.localeCompare(pb.category);
            default: return 0;
        }
    });

    cards.forEach(c => container.appendChild(c));
}

function populateCategorySelect() {
    const categorySelect = document.getElementById('category-select');
    if (!categorySelect) return;

    const categories = [...new Set(products.map(p => p.category))].filter(Boolean);
    categorySelect.innerHTML = `<option value="">Все категории</option>` +
        categories.map(cat => `<option value="${cat}">${cat}</option>`).join('');
}

// Authentication functions
function bindAuthForms() {
    const regForm = document.getElementById('register-form');
    if (regForm) {
        regForm.addEventListener('submit', async e => {
            e.preventDefault();
            const fd = new FormData(regForm);

            try {
                const resp = await fetch(regForm.action, { method: 'POST', body: fd });
                const ct = resp.headers.get('Content-Type') || '';

                if (ct.includes('application/json')) {
                    location.reload();
                    return;
                }

                const html = await resp.text();
                const modalContent = document.querySelector('#registerModal .modal-content');
                if (modalContent) {
                    modalContent.innerHTML = html;
                    bindAuthForms();
                }
            } catch (error) {
                console.error('Registration error:', error);
                showToast('Ошибка регистрации', 'error');
            }
        });
    }

    const loginForm = document.getElementById('login-form');
    if (loginForm) {
        loginForm.addEventListener('submit', async e => {
            e.preventDefault();
            const fd = new FormData(loginForm);

            try {
                const resp = await fetch(loginForm.action, { method: 'POST', body: fd });
                const ct = resp.headers.get('Content-Type') || '';

                if (ct.includes('application/json')) {
                    location.reload();
                    return;
                }

                const html = await resp.text();
                const modalContent = document.querySelector('#loginModal .modal-content');
                if (modalContent) {
                    modalContent.innerHTML = html;
                    bindAuthForms();
                }
            } catch (error) {
                console.error('Login error:', error);
                showToast('Ошибка входа', 'error');
            }
        });
    }

    // Bind validation if available
    document.querySelectorAll('#loginModal .modal-content, #registerModal .modal-content').forEach(c => {
        c.querySelectorAll('form').forEach(f => {
            if (window.jQuery && jQuery.validator && jQuery.validator.unobtrusive) {
                jQuery.validator.unobtrusive.parse(f);
            }
        });
    });
}

function openModal(url, selector) {
    fetch(url)
        .then(r => r.text())
        .then(html => {
            const modalContent = document.querySelector(`${selector} .modal-content`);
            if (modalContent) {
                modalContent.innerHTML = html;

                if (window.bootstrap) {
                    const modal = new bootstrap.Modal(document.querySelector(selector));
                    modal.show();
                }

                bindAuthForms();
            }
        })
        .catch(error => {
            console.error('Error loading modal:', error);
            showToast('Ошибка загрузки', 'error');
        });
}

// User profile functions
async function loadUserProfile() {
    try {
        const resp = await fetch('/Account/profile');
        if (!resp.ok) {
            if (resp.status === 401) {
                showToast('Необходимо войти в систему для просмотра профиля', 'warning');
                return;
            }
            throw new Error(`Status ${resp.status}`);
        }

        const vm = await resp.json();

        // Fill form fields
        const fields = ['firstName', 'lastName', 'email', 'phoneNumber', 'bio'];
        fields.forEach(field => {
            const element = document.getElementById(`profile-${field}`);
            if (element) {
                element.value = vm[field] || '';
            }
        });

        console.log('Профиль загружен:', vm);
    } catch (e) {
        console.error('Не удалось загрузить профиль:', e);

        showToast('Загружены демонстрационные данные', 'info');
    }
}

// Modal helper functions
function showLogin() {
    openModal('/Account/Login', '#loginModal');
}

function showRegister() {
    openModal('/Account/Register', '#registerModal');
}

// Enhanced profile save function
async function saveProfile() {
    const btn = document.getElementById('profile-save-btn');
    if (!btn) return;

    const originalContent = btn.innerHTML;

    // Show loading state
    btn.innerHTML = '<span class="loading-spinner me-2"></span>Сохранение...';
    btn.disabled = true;

    // Get form data
    const formData = {
        firstName: document.getElementById('profile-firstName')?.value.trim() || '',
        lastName: document.getElementById('profile-lastName')?.value.trim() || '',
        email: document.getElementById('profile-email')?.value.trim() || '',
        phoneNumber: document.getElementById('profile-phone')?.value.trim() || ''
    };

    try {
        const resp = await fetch('/Account/profile', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(formData)
        });

        if (resp.ok) {
            // Success animation
            const formContainer = document.querySelector('.profile-form-container');
            if (formContainer) {
                formContainer.classList.add('success-animation');
                setTimeout(() => {
                    formContainer.classList.remove('success-animation');
                }, 600);
            }

            showToast('Профиль успешно сохранён!', 'success');
        } else {
            const error = await resp.json();
            const msg = error.errors
                ? Object.values(error.errors).flat().join('; ')
                : error.title || resp.statusText;
            showToast('Ошибка: ' + msg, 'error');
        }
    } catch (e) {
        console.error('Ошибка сохранения профиля:', e);
        showToast('Произошла ошибка при сохранении профиля', 'error');
    } finally {
        // Restore button state
        btn.innerHTML = originalContent;
        btn.disabled = false;
    }
}

// Initialize everything when DOM is loaded
document.addEventListener('DOMContentLoaded', async function () {
    // Load initial data
    await loadProductsData();
    populateCategorySelect();
    updateCartUI();
    loadFeaturedProducts();
    showPage('home');

    // Bind navigation
    document.querySelectorAll('.nav-link').forEach(link => {
        link.addEventListener('click', e => {
            e.preventDefault();
            const href = link.getAttribute('href');
            if (href && href.startsWith('#')) {
                const page = href.substring(1);
                if (page) showPage(page);
            }
        });
    });

    // Bind search and filter controls
    const searchInput = document.getElementById('search-input');
    const categorySelect = document.getElementById('category-select');
    const sortSelect = document.getElementById('sort-select');

    if (searchInput) searchInput.addEventListener('input', filterProducts);
    if (categorySelect) categorySelect.addEventListener('change', filterProducts);
    if (sortSelect) sortSelect.addEventListener('change', sortProducts);

    // Bind checkout button
    const checkoutBtn = document.getElementById('checkout-btn');
    if (checkoutBtn) {
        checkoutBtn.addEventListener('click', () => {
            if (!cart.length) {
                showToast('Корзина пуста!', 'warning');
                return;
            }

            showToast('Заказ оформлен! Мы свяжемся с вами в ближайшее время.', 'success');
            cart = [];
            updateCartUI();

            // Hide cart modal if it exists
            const cartModal = document.getElementById('cartModal');
            if (cartModal && window.bootstrap) {
                const modalInstance = bootstrap.Modal.getInstance(cartModal);
                if (modalInstance) modalInstance.hide();
            }
        });
    }

    // Bind category cards
    document.querySelectorAll('.category-card').forEach(card => {
        card.addEventListener('click', () => showPage('products'));
    });

    // Bind profile save button
    const profileSaveBtn = document.getElementById('profile-save-btn');
    if (profileSaveBtn) {
        profileSaveBtn.addEventListener('click', saveProfile);
    }

    // Add input animations for profile form
    document.querySelectorAll('.form-control').forEach(input => {
        input.addEventListener('focus', function () {
            const parent = this.parentElement;
            if (parent) {
                parent.style.transform = 'scale(1.02)';
                parent.style.transition = 'transform 0.2s ease';
            }
        });

        input.addEventListener('blur', function () {
            const parent = this.parentElement;
            if (parent) {
                parent.style.transform = 'scale(1)';
            }
        });
    });

    // Add hover effects to profile cards
    document.querySelectorAll('.profile-card').forEach(card => {
        card.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(-5px) scale(1.02)';
            this.style.transition = 'transform 0.3s ease';
        });

        card.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });

    console.log('Application initialized successfully');
});