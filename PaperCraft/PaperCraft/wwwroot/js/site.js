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

let favorites = JSON.parse(localStorage.getItem('favorites') || '[]');


let currentUser = null;
let userStats = null;
let userActivity = [];
let userOrders = [];
let currentOrdersPage = 1;
const ordersPerPage = 5;


// Load products data from API
async function loadProductsData() {
    try {
        const resp = await fetch('/api/products');
        if (!resp.ok) throw new Error(`Status ${resp.status}`);
        products = await resp.json();
    } catch (err) {
        console.error('Failed to load products:', err);
        showToast('Не удалось загрузить товары', 'error');
    }
}

// Load and display products
function loadProducts() {
    const list = document.getElementById('product-list');
    if (!list) return;

    // Загружаем текущие избранные из localStorage
    const favorites = JSON.parse(localStorage.getItem('favorites') || '[]');

    list.innerHTML = products.map((p, i) => {
        const isFav = favorites.includes(p.id);
        return `
        <div class="col-lg-3 col-md-4 col-sm-6 mb-4"
             data-aos="zoom-in"
             data-aos-delay="${(i % 4) * 100}"
             data-product-id="${p.id}">
          <div class="card product-card h-100 border-0 rounded-4 shadow-sm position-relative">
            <div class="product-image overflow-hidden">
              <img src="${p.imageUrl}" class="img-fluid w-100" alt="${p.name}">
              <!-- Кнопка "избранное" -->
              <button class="btn btn-favorite position-absolute top-0 end-0 m-2 p-2"
                      onclick="toggleFavorite(${p.id})">
                <i class="${isFav
                ? 'fas fa-heart text-danger'
                : 'far fa-heart text-black'} fs-5"></i>
              </button>
            </div>
            <div class="card-body d-flex flex-column">
              <h5 class="card-title mb-2">${p.name}</h5>
              <p class="card-text text-muted mb-3">${p.description}</p>
              <div class="mt-auto d-flex justify-content-between align-items-center">
                <span class="fw-bold fs-5">${p.price.toLocaleString()} ₽</span>
                <button class="btn btn-sm btn-outline-primary"
                        onclick="addToCart(${p.id})">
                  Купить
                </button>
              </div>
            </div>
          </div>
        </div>`;
    }).join('');
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

// Update cart UI
function updateCartUI() {
    const countEl = document.getElementById('cart-count');
    const itemsEl = document.getElementById('cart-items');
    const totalEl = document.getElementById('cart-total');
    if (!countEl || !itemsEl || !totalEl) return;
    const totalCount = cart.reduce((s, x) => s + x.quantity, 0);
    const totalPrice = cart.reduce((s, x) => s + x.quantity * x.price, 0);
    countEl.textContent = totalCount;
    totalEl.textContent = totalPrice.toLocaleString() + ' ₽';
    if (!cart.length) {
        itemsEl.innerHTML = `
      <div class="text-center py-5">
        <i class="fas fa-shopping-cart" style="font-size:4rem;color:#ddd"></i>
        <p class="mt-3">Корзина пуста</p>
      </div>`;
    } else {
        itemsEl.innerHTML = cart.map(item => `
      <div class="cart-item d-flex justify-content-between align-items-center mb-3 p-3 border rounded">
        <div class="d-flex align-items-center">
          <img src="${item.imageUrl}" class="me-3 rounded" style="width:60px;height:60px;object-fit:cover" alt="">
          <div><h6 class="mb-0">${item.name}</h6><small class="text-muted">${item.price.toLocaleString()} ₽</small></div>
        </div>
        <div class="d-flex align-items-center">
          <button class="btn btn-sm btn-outline-secondary" onclick="updateQuantity(${item.id},-1)"><i class="fas fa-minus"></i></button>
          <span class="mx-2 fw-bold">${item.quantity}</span>
          <button class="btn btn-sm btn-outline-secondary" onclick="updateQuantity(${item.id},1)"><i class="fas fa-plus"></i></button>
          <button class="btn btn-sm btn-outline-danger ms-3" onclick="removeFromCart(${item.id})"><i class="fas fa-trash"></i></button>
        </div>
      </div>
    `).join('');
    }
}

/**
 * Добавляет товар в корзину по его id
 * @param {number} productId
 */
function addToCart(productId) {
    // Найти товар в списке products
    const prod = products.find(p => p.id === productId);
    if (!prod) {
        console.warn(`Product ${productId} not found`);
        return;
    }
    // Проверить, есть ли уже в cart
    const existing = cart.find(item => item.id === productId);
    if (existing) {
        existing.quantity++;
    } else {
        // Копируем все поля товара + количество
        cart.push({ ...prod, quantity: 1 });
    }
    // Обновить отображение корзины
    updateCartUI();
    // Уведомление
    showToast('Товар добавлен в корзину!', 'success');
}


// Enhanced toast function
function showToast(message, type = 'success') {
    const container = document.querySelector('.toast-container');
    if (!container) return;

    const toast = document.createElement('div');
    toast.className = 'custom-toast p-3';

    const iconClass = {
        success: 'fas fa-check-circle text-success',
        error: 'fas fa-exclamation-circle text-danger',
        warning: 'fas fa-exclamation-triangle text-warning',
        info: 'fas fa-info-circle text-info'
    }[type] || '';

    toast.innerHTML = `
    <div class="d-flex align-items-center">
      <i class="${iconClass} me-2 fs-5"></i>
      <span class="fw-semibold">${message}</span>
      <button type="button" class="btn-close ms-auto" aria-label="Close"></button>
    </div>
  `;

    // Закрытие по клику
    toast.querySelector('.btn-close').addEventListener('click', () => {
        toast.remove();
    });

    container.appendChild(toast);

    setTimeout(() => {
        toast.remove();
    }, 5000);
}


// Page navigation
function showPage(page) {
    document.querySelectorAll('.page-content').forEach(el => el.classList.add('d-none'));
    const target = document.getElementById(`${page}-page`);
    if (target) target.classList.remove('d-none');
    currentPage = page;
    document.querySelectorAll('.nav-link').forEach(l => l.classList.remove('active'));
    const active = document.querySelector(`.nav-link[href="#${page}"]`);
    if (active) active.classList.add('active');
    if (page === 'profile') loadUserProfile();
    window.scrollTo(0, 0);
    AOS.refresh();
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

    const changeForm = document.getElementById('change-password-form');
    if (changeForm) {
        changeForm.addEventListener('submit', async function (e) {
            e.preventDefault();
            const url = this.action;
            const formData = new FormData(this);
            const resp = await fetch(url, { method: 'POST', body: formData });
            if (resp.headers.get('Content-Type').includes('application/json')) {
                // успешная смена
                bootstrap.Modal.getInstance(document.getElementById('changePasswordModal')).hide();
                showToast('Пароль успешно изменён', 'success');
            } else {
                // возвращён partial с ошибками
                const html = await resp.text();
                document.querySelector('#changePasswordModal .modal-content').innerHTML = html;
                bindAuthForms();
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

function updateProfileForm() {
    if (!currentUser) return;
    document.getElementById('profile-firstName').value = currentUser.firstName;
    document.getElementById('profile-lastName').value = currentUser.lastName;
    document.getElementById('profile-email').value = currentUser.email;
    document.getElementById('profile-phone').value = currentUser.phoneNumber;
    document.getElementById('profile-bio').value = currentUser.bio;
}

function updateStatsDisplay() {
    if (!userStats) return;
    document.getElementById('stat-orders').textContent = userStats.totalOrders;
    document.getElementById('stat-years').textContent = userStats.yearsWithUs;
    document.getElementById('stat-rating').textContent = `★ ${userStats.rating}`;
    document.getElementById('stat-bonus').textContent = userStats.bonusPoints.toLocaleString();
}

function updateActivityTimeline() {
    const timeline = document.getElementById('activity-timeline');
    if (!timeline) return;

    const maxItems = 3;
    const toShow = userActivity.slice(0, maxItems);

    if (toShow.length === 0) {
        timeline.innerHTML = `
            <div class="text-center py-3 text-muted">
                <i class="fas fa-info-circle me-2"></i>
                Нет активности для отображения
            </div>`;
        return;
    }

    timeline.innerHTML = toShow.map(a => `
        <div class="activity-item d-flex align-items-center mb-3" data-aos="fade-up">
          <i class="fas fa-history me-2"></i>
          <div>
            <div class="fw-semibold text-dark">${a.action}</div>
            <div class="small text-muted">${a.description} • ${a.timeAgo}</div>
          </div>
        </div>
    `).join('');

    AOS.refresh();
}


function getActivityIcon(type) {
    switch (type) {
        case 'Login': return 'fas fa-sign-in-alt text-success';
        case 'Registration': return 'fas fa-user-plus text-primary';
        case 'Order': return 'fas fa-shopping-bag text-warning';
        case 'ProfileUpdate': return 'fas fa-user-edit text-info';
        default: return 'fas fa-circle text-secondary';
    }
}

// Modal helper functions
function showLogin() {
    openModal('/Account/Login', '#loginModal');
}

function showRegister() {
    openModal('/Account/Register', '#registerModal');
}

async function loadUserOrders(page = 1) {
    try {
        const response = await fetch(`/Account/GetOrders?page=${page}&pageSize=${ordersPerPage}`);

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }

        const data = await response.json();
        displayOrders(data.orders);
        updateOrdersPagination(data.totalPages, page);

    } catch (error) {
        console.error('Ошибка загрузки заказов:', error);
        // Show demo orders if API fails
        displayOrders(userOrders);
    }
}

function updateOrdersPagination(totalPages, currentPage) {
    const pagination = document.getElementById('orders-pagination');
    if (!pagination) return; 
    const paginationList = pagination.querySelector('ul');

    if (totalPages <= 1) {
        pagination.classList.add('d-none');
        return;
    }

    pagination.classList.remove('d-none');

    let paginationHTML = '';

    // Previous button
    if (currentPage > 1) {
        paginationHTML += `
                    <li class="page-item">
                        <a class="page-link" href="#" onclick="loadUserOrders(${currentPage - 1}); return false;">
                            <i class="fas fa-chevron-left"></i>
                        </a>
                    </li>`;
    }

    // Page numbers
    const startPage = Math.max(1, currentPage - 2);
    const endPage = Math.min(totalPages, currentPage + 2);

    if (startPage > 1) {
        paginationHTML += `<li class="page-item"><a class="page-link" href="#" onclick="loadUserOrders(1); return false;">1</a></li>`;
        if (startPage > 2) {
            paginationHTML += `<li class="page-item disabled"><span class="page-link">...</span></li>`;
        }
    }

    for (let i = startPage; i <= endPage; i++) {
        paginationHTML += `
                    <li class="page-item ${i === currentPage ? 'active' : ''}">
                        <a class="page-link" href="#" onclick="loadUserOrders(${i}); return false;">${i}</a>
                    </li>`;
    }

    if (endPage < totalPages) {
        if (endPage < totalPages - 1) {
            paginationHTML += `<li class="page-item disabled"><span class="page-link">...</span></li>`;
        }
        paginationHTML += `<li class="page-item"><a class="page-link" href="#" onclick="loadUserOrders(${totalPages}); return false;">${totalPages}</a></li>`;
    }

    // Next button
    if (currentPage < totalPages) {
        paginationHTML += `
                    <li class="page-item">
                        <a class="page-link" href="#" onclick="loadUserOrders(${currentPage + 1}); return false;">
                            <i class="fas fa-chevron-right"></i>
                        </a>
                    </li>`;
    }

    paginationList.innerHTML = paginationHTML;
}


function displayOrders(orders) {
    const container = document.getElementById('orders-container');
    if (!orders || !orders.length) {
        container.innerHTML = `
          <div class="text-center py-5">
            <i class="fas fa-shopping-bag fs-1 text-muted mb-3"></i>
            <h5 class="text-white">У вас пока нет заказов</h5>
            <p class="text-muted">Начните покупки в нашем каталоге!</p>
            <button class="btn btn-gradient rounded-3 mt-3" onclick="window.location.href='/Products'">
              <i class="fas fa-shopping-cart me-2"></i>Перейти к покупкам
            </button>
          </div>`;
        return;
    }

    const ordersHTML = orders.map(order => {
        // используем camelCase-поля, а не PascalCase
        const id = order.id;
        const date = formatDate(order.orderDate);
        const statusText = order.statusText;
        const badgeClass = order.status;
        const itemCount = order.itemCount;
        const totalAmount = order.totalAmount.toLocaleString();
        const tracking = order.trackingNumber;

        return `
        <div class="order-card rounded-3 p-3 mb-3" data-aos="fade-up">
          <div class="column align-items-center">
            <div class="col-md-2">
              <div class="fw-bold text-muted">#${id}</div>
              <small class="text-muted">${date}</small>
            </div>
            <div class="col-md-3">
              <span class="${badgeClass}">${statusText}</span>
            </div>
            <div class="row-md-2">
              <div class="text-muted">${itemCount} товар${getPlural(itemCount)}</div>
            </div>
            <div class="row-md-3">
              <div class="fw-bold text-muted fs-5">${totalAmount} ₽</div>
            </div>
          </div>
          ${tracking ? `
            <div class="mt-2 pt-2 border-top border-secondary">
              <small class="text-muted">
                <i class="fas fa-truck me-1"></i>Трек-номер: ${tracking}
              </small>
            </div>` : ''}
        </div>`;
    }).join('');

    container.innerHTML = ordersHTML;
}

function exportUserData() {
    if (!currentUser) {
        showToast('Нет данных для экспорта', 'warning');
        return;
    }

    const userData = {
        profile: currentUser,
        stats: userStats,
        activity: userActivity,
        orders: userOrders,
        exportDate: new Date().toISOString()
    };

    const dataStr = JSON.stringify(userData, null, 2);
    const dataBlob = new Blob([dataStr], { type: 'application/json' });

    const link = document.createElement('a');
    link.href = URL.createObjectURL(dataBlob);
    link.download = `profile_data_${new Date().toISOString().split('T')[0]}.json`;
    link.click();

    showToast('Данные профиля экспортированы', 'success');
}


function showOrders() {
    showToast('Переход на страницу всех заказов (функция в разработке)', 'info');
}

function showOrderDetails(orderId) {
    // This would typically open a modal with order details
    showToast(`Детали заказа #${orderId} (функция в разработке)`, 'info');
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('ru-RU', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
    });
}

function getPlural(count) {
    if (count % 10 === 1 && count % 100 !== 11) return '';
    if ([2, 3, 4].includes(count % 10) && ![12, 13, 14].includes(count % 100)) return 'а';
    return 'ов';
}

// Enhanced profile save function
async function saveProfile() {
    const saveBtn = document.getElementById('profile-save-btn');
    const originalText = saveBtn.innerHTML;

    // Show loading state
    saveBtn.innerHTML = '<div class="loading-spinner me-2"></div>Сохранение...';
    saveBtn.disabled = true;

    try {
        const profileData = {
            FirstName: document.getElementById('profile-firstName').value.trim(),
            LastName: document.getElementById('profile-lastName').value.trim(),
            Email: document.getElementById('profile-email').value.trim(),
            PhoneNumber: document.getElementById('profile-phone').value.trim(),
            Bio: document.getElementById('profile-bio').value.trim()
        };

        // Basic validation
        if (!profileData.FirstName || !profileData.LastName || !profileData.Email) {
            showToast('Заполните обязательные поля: Имя, Фамилия, Email', 'error');
            return;
        }

        // Email validation
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(profileData.Email)) {
            showToast('Введите корректный email адрес', 'error');
            return;
        }

        const response = await fetch('/Account/Profile', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(profileData)
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || `HTTP ${response.status}`);
        }

        // Update current user data
        currentUser = { ...currentUser, ...profileData };

        // Show success animation
        const formContainer = document.querySelector('.profile-form-container');
        formContainer.classList.add('success-animation');
        setTimeout(() => formContainer.classList.remove('success-animation'), 600);

        showToast('Профиль успешно обновлен!', 'success');

        // Reload activity to show profile update
        setTimeout(() => {
            loadUserProfile();
        }, 1000);

    } catch (error) {
        console.error('Ошибка сохранения профиля:', error);
        showToast('Ошибка при сохранении профиля: ' + error.message, 'error');
    } finally {
        // Restore button
        saveBtn.innerHTML = originalText;
        saveBtn.disabled = false;
    }
}

function resetProfileForm() {
    if (!currentUser) return;

    if (confirm('Отменить все изменения и вернуть исходные данные?')) {
        updateProfileForm();
        showToast('Изменения отменены', 'info');
    }
}

function refreshOrders() {
    const container = document.getElementById('orders-container');
    container.innerHTML = `
                <div class="text-center py-4">
                    <div class="loading-spinner me-2"></div>
                    Обновление заказов...
                </div>`;

    setTimeout(() => {
        loadUserOrders(currentOrdersPage);
    }, 500);
}




// Handle page visibility change
document.addEventListener('visibilitychange', function () {
    if (!document.hidden && currentUser) {
        // Refresh data when user returns to page
        setTimeout(() => {
            loadUserProfile();
        }, 1000);
    }
});

// Load user profile data
async function loadUserProfile() {
    try {
        const resp = await fetch('/Account/Profile');
        if (!resp.ok) throw new Error(resp.status);
        const data = await resp.json();
        currentUser = data.profile;
        userStats = data.stats;
        userActivity = data.recentActivity;
        userOrders = data.recentOrders;
        updateProfileForm();
        updateStatsDisplay();
        updateActivityTimeline();
        displayOrders(userOrders);
        updateOrdersPagination(Math.ceil(data.totalCount / ordersPerPage), 1);
    } catch (err) {
        console.error('Failed to load profile:', err);
        showToast('Ошибка загрузки профиля', 'error');
    }
}

// Phone number formatting
document.getElementById('profile-phone').addEventListener('input', function (e) {
    let value = e.target.value.replace(/\D/g, '');
    if (value.startsWith('7')) {
        value = value.substring(1);
    }
    if (value.length > 0) {
        if (value.length <= 3) {
            value = `+7 (${value}`;
        } else if (value.length <= 6) {
            value = `+7 (${value.substring(0, 3)}) ${value.substring(3)}`;
        } else if (value.length <= 8) {
            value = `+7 (${value.substring(0, 3)}) ${value.substring(3, 6)}-${value.substring(6)}`;
        } else {
            value = `+7 (${value.substring(0, 3)}) ${value.substring(3, 6)}-${value.substring(6, 8)}-${value.substring(8, 10)}`;
        }
    }
    e.target.value = value;
});

// Global error handler
window.addEventListener('error', function (e) {
    console.error('JavaScript Error:', e.error);
    showToast('Произошла ошибка на странице', 'error');
});

// Unhandled promise rejection handler
window.addEventListener('unhandledrejection', function (e) {
    console.error('Unhandled Promise Rejection:', e.reason);
    showToast('Ошибка при выполнении запроса', 'error');
});

function showChangePassword() {
    openModal('/Account/ChangePassword', '#changePasswordModal');
}

function saveFavorites() {
    localStorage.setItem('favorites', JSON.stringify(favorites));
}

/**
* Переключает статус избранного для товара id
*/
// Переключить избранное
function toggleFavorite(productId) {
    // Читаем массив из localStorage
    let favorites = JSON.parse(localStorage.getItem('favorites') || '[]');

    // Добавляем или удаляем из массива
    if (favorites.includes(productId)) {
        favorites = favorites.filter(id => id !== productId);
    } else {
        favorites.push(productId);
    }

    // Сохраняем обратно
    localStorage.setItem('favorites', JSON.stringify(favorites));

    // Находим иконку в DOM
    const icon = document.querySelector(
        `[data-product-id="${productId}"] .btn-favorite i`
    );
    if (!icon) return;

    // Переключаем классы
    if (favorites.includes(productId)) {
        icon.classList.remove('far', 'text-black');
        icon.classList.add('fas', 'text-danger');
    } else {
        icon.classList.remove('fas', 'text-danger');
        icon.classList.add('far', 'text-black');
    }
}


/**
 * Обновляет иконку «сердечко» на карточке товара
 */
function updateFavoriteIcon(id) {
    const card = document.querySelector(`[data-product-id="${id}"]`);
    if (!card) return;

    const icon = card.querySelector('.btn-favorite i');
    const isFav = favorites.includes(id);

    icon.classList.toggle('fas', isFav);
    icon.classList.toggle('far', !isFav);
    icon.classList.toggle('text-danger', isFav);
    icon.classList.toggle('text-white', !isFav);
}

// Если карточки уже отрендерены, можно вызвать обновление для всех:
function refreshAllFavorites() {
    favorites.forEach(updateFavoriteIcon);
}

/**
 * Открыть модалку «Избранное»
 */
function showFavorites() {
    // Берём список ID из localStorage
    const favorites = JSON.parse(localStorage.getItem('favorites') || '[]');
    const container = document.getElementById('favorites-list');
    const emptyMsg = document.getElementById('no-favorites');

    // Очищаем
    container.innerHTML = '';

    if (!favorites.length) {
        emptyMsg.classList.remove('d-none');
    } else {
        emptyMsg.classList.add('d-none');
        // Генерируем карточки
        favorites.forEach((id, idx) => {
            const p = products.find(x => x.id === id);
            if (!p) return;

            const col = document.createElement('div');
            col.className = 'col-lg-4 col-md-6';

            col.innerHTML = `
        <div class="card h-100 shadow-sm position-relative" data-aos="fade-up" data-aos-delay="${(idx % 3) * 100}">
          <img src="${p.imageUrl}" class="card-img-top" alt="${p.name}">
          <div class="card-body d-flex flex-column">
            <h6 class="card-title">${p.name}</h6>
            <p class="card-text text-truncate">${p.description}</p>
            <div class="mt-auto d-flex justify-content-between align-items-center">
              <span class="fw-bold">${p.price.toLocaleString()} ₽</span>
              <button class="btn btn-sm btn-primary"
                      onclick="addToCart(${p.id}); showToast('Добавлено в корзину', 'success');">
                В корзину
              </button>
            </div>
          </div>
          <button class="btn btn-sm btn-outline-danger position-absolute top-0 end-0 m-2 p-1"
                  title="Убрать из избранного"
                  onclick="toggleFavorite(${p.id}); showFavorites();">
            <i class="fas fa-heart"></i>
          </button>
        </div>`;
            container.appendChild(col);
        });
        // реинициализируем анимации
        AOS.refresh();
    }

    // Показываем модалку
    const favModal = new bootstrap.Modal(document.getElementById('favoritesModal'));
    favModal.show();
}

// Привязка кнопки «Избранное» в профиль
document.querySelectorAll('.btn-favorite-sidebar').forEach(btn =>
    btn.addEventListener('click', showFavorites)
);

/**
 * Открывает модалку корзины и показывает форму офорления внутри.
 */
function showCartModal() {
    // 1) Собираем HTML списка товаров из cart
    const itemsHtml = cart.length
        ? cart.map(i => `
        <div class="d-flex justify-content-between mb-2">
          <div>${i.name} × ${i.quantity}</div>
          <div>${(i.price * i.quantity).toLocaleString()} ₽</div>
        </div>
      `).join('')
        : `<div class="text-center py-5 text-muted">
         <i class="fas fa-shopping-cart fa-3x mb-3"></i>
         <p>В вашей корзине пока нет товаров.</p>
       </div>`;

    // 2) Собираем итоговую сумму
    const total = cart.reduce((sum, i) => sum + i.price * i.quantity, 0);

    // 3) Строим полный шаблон модалки
    const html = `
    <div class="modal-header">
      <h5 class="modal-title"><i class="fas fa-shopping-cart me-2"></i>Корзина покупок</h5>
      <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
    </div>
    <div class="modal-body">
      <div id="cart-items-list" class="mb-3">${itemsHtml}</div>
      <div class="mb-3">
        <label for="shippingAddress" class="form-label">Адрес доставки</label>
        <textarea id="shippingAddress" class="form-control" rows="2"></textarea>
      </div>
    </div>
    <div class="modal-footer d-flex justify-content-between align-items-center">
      <strong>Итого: ${total.toLocaleString()} ₽</strong>
      <div>
        <button type="button" class="btn btn-secondary me-2" data-bs-dismiss="modal">Отмена</button>
        <button type="button" class="btn btn-primary" onclick="placeOrder()">Оформить заказ</button>
      </div>
    </div>
  `;

    // 4) Вставляем и показываем модалку
    document.getElementById('cartModalContent').innerHTML = html;
    new bootstrap.Modal(document.getElementById('cartModal')).show();
}

// Отправка заказа
async function placeOrder() {
    const address = document.getElementById('shippingAddress').value.trim();
    if (!address) {
        showToast('Пожалуйста, укажите адрес доставки', 'warning');
        return;
    }
    if (!cart.length) {
        showToast('Корзина пуста', 'warning');
        return;
    }

    const vm = {
        shippingAddress: address,
        items: cart.map(i => ({
            productId: i.id,
            quantity: i.quantity,
            price: i.price
        }))
    };

    try {
        const resp = await fetch('/Order/PlaceOrder', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(vm)
        });

        if (!resp.ok) {
            // если сервер вернул 400/500 — попробуем прочитать текст
            const errText = await resp.text();
            throw new Error(errText || `HTTP ${resp.status}`);
        }

        // здесь точно JSON
        const data = await resp.json();

        if (data.success) {
            bootstrap.Modal.getInstance(document.getElementById('cartModal')).hide();
            cart = [];
            updateCartUI();
            showToast(`Заказ №${data.orderId} успешно оформлен`, 'success');
        } else {
            throw new Error(data.error || 'Не удалось оформить заказ');
        }
    } catch (err) {
        console.error('Ошибка при оформлении заказа:', err);
        showToast(err.message || 'Ошибка при оформлении заказа', 'error');
    }
}


// Показать мои заказы
async function showMyOrders() {
    const html = await fetch('/Order/MyOrders').then(r => r.text());
    document.getElementById('myOrdersModalContent').innerHTML = html;
    new bootstrap.Modal(document.getElementById('myOrdersModal')).show();
}

// Привязываем кнопки в профиле
//document.querySelector('#profile-favorites-btn')
//    .addEventListener('click', showMyOrders);



// Initialize everything when DOM is loaded
document.addEventListener('DOMContentLoaded', async function () {
    // Load initial data
    await loadProductsData();
    loadProducts();
    updateCartUI();
    refreshAllFavorites();
    showPage('home');
    document.querySelectorAll('.nav-link').forEach(l => l.addEventListener('click', e => { e.preventDefault(); showPage(e.target.getAttribute('href').substring(1)); }));

    // Bind search and filter controls
    const searchInput = document.getElementById('search-input');
    const categorySelect = document.getElementById('category-select');
    const sortSelect = document.getElementById('sort-select');
    if (searchInput) searchInput.addEventListener('input', filterProducts);
    if (categorySelect) categorySelect.addEventListener('change', filterProducts);
    if (sortSelect) sortSelect.addEventListener('change', sortProducts);

    // Bind category cards
    document.querySelectorAll('.category-card').forEach(card => {
        card.addEventListener('click', () => showPage('products'));
    });

    // Bind profile save button
    const profileSaveBtn = document.getElementById('profile-save-btn');
    if (profileSaveBtn) {
        profileSaveBtn.addEventListener('click', saveProfile);
    }

    // Form inputs validation animations
    const formInputs = document.querySelectorAll('#profile-form input, #profile-form textarea, .form-control');
    formInputs.forEach(input => {
        input.addEventListener('focus', function () {
            const parent = this.parentElement;
            parent?.style.setProperty('transform', 'scale(1.02)');
            parent?.style.setProperty('transition', 'transform 0.2s ease');
        });
        input.addEventListener('blur', function () {
            this.parentElement?.style.setProperty('transform', 'scale(1)');
        });
        input.addEventListener('input', function () {
            this.classList.remove('is-invalid');
            if (this.value.trim()) {
                this.classList.add('is-valid');
            } else {
                this.classList.remove('is-valid');
            }
        });
    });

    // Profile-card hover effects
    document.querySelectorAll('.profile-card').forEach(card => {
        card.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(-5px) scale(1.02)';
            this.style.transition = 'transform 0.3s ease';
        });
        card.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });

    // Keyboard shortcuts
    document.addEventListener('keydown', function (e) {
        // Ctrl+S to save profile
        if (e.ctrlKey && e.key === 's') {
            e.preventDefault();
            saveProfile();
        }
        // Escape to close toasts
        if (e.key === 'Escape') {
            document.querySelectorAll('.custom-toast').forEach(toast => toast.remove());
        }
    });

    // Auto-hide loading spinners
    setTimeout(() => {
        document.querySelectorAll('.loading-spinner').forEach(spinner => {
            const parent = spinner.closest('.text-center');
            if (parent && parent.textContent.includes('Загрузка')) {
                parent.innerHTML = `
                    <div class="text-muted">
                        <i class="fas fa-exclamation-circle me-2"></i>
                        Не удалось загрузить данные
                    </div>`;
            }
        });
    }, 10000);

    console.log('Application initialized successfully');
});