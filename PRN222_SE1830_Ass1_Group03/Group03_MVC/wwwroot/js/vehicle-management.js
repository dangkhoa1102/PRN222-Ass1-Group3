// ===== VEHICLE MANAGEMENT JAVASCRIPT =====

class VehicleManager {
    constructor() {
        this.init();
    }

    init() {
        this.initVehicleCards();
        this.initVehicleFilters();
        this.initVehicleSearch();
        this.initVehicleActions();
        this.initVehicleForms();
        this.initVehicleModals();
        this.initVehiclePagination();
    }

    // ===== VEHICLE CARDS =====
    initVehicleCards() {
        const vehicleCards = document.querySelectorAll('.vehicle-card');
        
        vehicleCards.forEach(card => {
            // Add hover effects
            card.addEventListener('mouseenter', () => {
                card.style.transform = 'translateY(-8px)';
                card.style.boxShadow = '0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)';
            });

            card.addEventListener('mouseleave', () => {
                card.style.transform = 'translateY(0)';
                card.style.boxShadow = '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)';
            });

            // Add click to view details
            const viewButton = card.querySelector('.btn-primary');
            if (viewButton) {
                viewButton.addEventListener('click', (e) => {
                    e.preventDefault();
                    const vehicleId = card.dataset.vehicleId;
                    if (vehicleId) {
                        window.location.href = `/Vehicle/Details/${vehicleId}`;
                    }
                });
            }
        });
    }

    // ===== VEHICLE FILTERS =====
    initVehicleFilters() {
        const filterForm = document.querySelector('.vehicle-filters');
        if (!filterForm) return;

        const filterInputs = filterForm.querySelectorAll('select, input');
        
        filterInputs.forEach(input => {
            input.addEventListener('change', () => {
                this.applyFilters();
            });
        });
    }

    applyFilters() {
        const form = document.querySelector('.vehicle-filters');
        if (!form) return;

        const formData = new FormData(form);
        const params = new URLSearchParams();
        
        for (let [key, value] of formData.entries()) {
            if (value) {
                params.append(key, value);
            }
        }

        // Update URL without page reload
        const newUrl = `${window.location.pathname}?${params.toString()}`;
        window.history.pushState({}, '', newUrl);

        // Apply filters to vehicle cards
        this.filterVehicleCards(formData);
    }

    filterVehicleCards(formData) {
        const vehicleCards = document.querySelectorAll('.vehicle-card');
        
        vehicleCards.forEach(card => {
            let showCard = true;
            
            // Check each filter
            for (let [key, value] of formData.entries()) {
                if (value) {
                    const cardValue = card.dataset[key];
                    if (cardValue && !cardValue.toLowerCase().includes(value.toLowerCase())) {
                        showCard = false;
                        break;
                    }
                }
            }
            
            card.style.display = showCard ? 'block' : 'none';
        });
    }

    // ===== VEHICLE SEARCH =====
    initVehicleSearch() {
        const searchInput = document.querySelector('.vehicle-search input');
        if (!searchInput) return;

        // Debounce search
        const debouncedSearch = debounce((query) => {
            this.searchVehicles(query);
        }, 300);

        searchInput.addEventListener('input', (e) => {
            debouncedSearch(e.target.value);
        });
    }

    searchVehicles(query) {
        const vehicleCards = document.querySelectorAll('.vehicle-card');
        
        vehicleCards.forEach(card => {
            const text = card.textContent.toLowerCase();
            const match = text.includes(query.toLowerCase());
            
            card.style.display = match ? 'block' : 'none';
            
            if (match && query) {
                this.highlightSearchText(card, query);
            }
        });
    }

    highlightSearchText(element, query) {
        const regex = new RegExp(`(${query})`, 'gi');
        const text = element.textContent;
        element.innerHTML = text.replace(regex, '<mark class="bg-warning">$1</mark>');
    }

    // ===== VEHICLE ACTIONS =====
    initVehicleActions() {
        // Edit buttons
        const editButtons = document.querySelectorAll('.btn-edit');
        editButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                e.preventDefault();
                const vehicleId = button.dataset.vehicleId;
                if (vehicleId) {
                    window.location.href = `/Vehicle/Edit/${vehicleId}`;
                }
            });
        });

        // Delete buttons
        const deleteButtons = document.querySelectorAll('.btn-delete');
        deleteButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                e.preventDefault();
                const vehicleId = button.dataset.vehicleId;
                const vehicleName = button.dataset.vehicleName;
                
                if (vehicleId) {
                    this.confirmDelete(vehicleId, vehicleName);
                }
            });
        });

        // View details buttons
        const viewButtons = document.querySelectorAll('.btn-view');
        viewButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                e.preventDefault();
                const vehicleId = button.dataset.vehicleId;
                if (vehicleId) {
                    window.location.href = `/Vehicle/Details/${vehicleId}`;
                }
            });
        });
    }

    confirmDelete(vehicleId, vehicleName) {
        if (window.confirmAction) {
            window.confirmAction(
                `Bạn có chắc chắn muốn xóa xe "${vehicleName}"? Hành động này không thể hoàn tác.`,
                () => {
                    this.deleteVehicle(vehicleId);
                }
            );
        } else {
            if (confirm(`Bạn có chắc chắn muốn xóa xe "${vehicleName}"?`)) {
                this.deleteVehicle(vehicleId);
            }
        }
    }

    async deleteVehicle(vehicleId) {
        try {
            const response = await fetch(`/Vehicle/Delete/${vehicleId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                }
            });

            if (response.ok) {
                if (window.showToast) {
                    window.showToast('Xe đã được xóa thành công!', 'success');
                }
                
                // Remove the card from DOM
                const card = document.querySelector(`[data-vehicle-id="${vehicleId}"]`);
                if (card) {
                    card.style.transition = 'all 0.3s ease';
                    card.style.opacity = '0';
                    card.style.transform = 'scale(0.8)';
                    
                    setTimeout(() => {
                        card.remove();
                    }, 300);
                }
            } else {
                throw new Error('Xóa xe thất bại');
            }
        } catch (error) {
            console.error('Error deleting vehicle:', error);
            if (window.showToast) {
                window.showToast('Có lỗi xảy ra khi xóa xe!', 'danger');
            }
        }
    }

    // ===== VEHICLE FORMS =====
    initVehicleForms() {
        const forms = document.querySelectorAll('.vehicle-form');
        
        forms.forEach(form => {
            // Real-time validation
            const inputs = form.querySelectorAll('input, select, textarea');
            inputs.forEach(input => {
                input.addEventListener('blur', () => {
                    this.validateField(input);
                });

                input.addEventListener('input', () => {
                    this.clearFieldError(input);
                });
            });

            // Form submission
            form.addEventListener('submit', (e) => {
                if (!this.validateForm(form)) {
                    e.preventDefault();
                }
            });
        });
    }

    validateField(field) {
        const value = field.value.trim();
        const fieldName = field.name;
        let isValid = true;
        let errorMessage = '';

        // Required field validation
        if (field.hasAttribute('required') && !value) {
            isValid = false;
            errorMessage = `${this.getFieldLabel(field)} là bắt buộc.`;
        }

        // Specific field validations
        switch (fieldName) {
            case 'Price':
                if (value && (isNaN(value) || parseFloat(value) <= 0)) {
                    isValid = false;
                    errorMessage = 'Giá xe phải là số dương.';
                }
                break;
            case 'Year':
                if (value && (isNaN(value) || parseInt(value) < 1900 || parseInt(value) > new Date().getFullYear() + 1)) {
                    isValid = false;
                    errorMessage = 'Năm sản xuất không hợp lệ.';
                }
                break;
            case 'Mileage':
                if (value && (isNaN(value) || parseInt(value) < 0)) {
                    isValid = false;
                    errorMessage = 'Số km không hợp lệ.';
                }
                break;
        }

        this.showFieldError(field, isValid, errorMessage);
        return isValid;
    }

    validateForm(form) {
        const inputs = form.querySelectorAll('input, select, textarea');
        let isFormValid = true;

        inputs.forEach(input => {
            if (!this.validateField(input)) {
                isFormValid = false;
            }
        });

        return isFormValid;
    }

    showFieldError(field, isValid, message) {
        const errorElement = field.parentElement.querySelector('.invalid-feedback');
        
        if (isValid) {
            field.classList.remove('is-invalid');
            field.classList.add('is-valid');
            if (errorElement) {
                errorElement.textContent = '';
            }
        } else {
            field.classList.remove('is-valid');
            field.classList.add('is-invalid');
            if (errorElement) {
                errorElement.textContent = message;
            }
        }
    }

    clearFieldError(field) {
        field.classList.remove('is-invalid', 'is-valid');
        const errorElement = field.parentElement.querySelector('.invalid-feedback');
        if (errorElement) {
            errorElement.textContent = '';
        }
    }

    getFieldLabel(field) {
        const label = field.parentElement.querySelector('label');
        return label ? label.textContent : field.name;
    }

    // ===== VEHICLE MODALS =====
    initVehicleModals() {
        const modals = document.querySelectorAll('.vehicle-modal');
        
        modals.forEach(modal => {
            modal.addEventListener('show.bs.modal', () => {
                // Load vehicle data if editing
                const vehicleId = modal.dataset.vehicleId;
                if (vehicleId) {
                    this.loadVehicleData(vehicleId, modal);
                }
            });
        });
    }

    async loadVehicleData(vehicleId, modal) {
        try {
            const response = await fetch(`/Vehicle/GetVehicleData/${vehicleId}`);
            if (response.ok) {
                const data = await response.json();
                this.populateForm(modal, data);
            }
        } catch (error) {
            console.error('Error loading vehicle data:', error);
        }
    }

    populateForm(modal, data) {
        const form = modal.querySelector('form');
        if (!form) return;

        Object.keys(data).forEach(key => {
            const field = form.querySelector(`[name="${key}"]`);
            if (field) {
                field.value = data[key];
            }
        });
    }

    // ===== VEHICLE PAGINATION =====
    initVehiclePagination() {
        const pagination = document.querySelector('.vehicle-pagination');
        if (!pagination) return;

        const pageLinks = pagination.querySelectorAll('.page-link');
        pageLinks.forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                const page = link.dataset.page;
                if (page) {
                    this.goToPage(page);
                }
            });
        });
    }

    goToPage(page) {
        const url = new URL(window.location);
        url.searchParams.set('page', page);
        window.location.href = url.toString();
    }

    // ===== VEHICLE EXPORT =====
    exportVehicleData(format) {
        const form = document.querySelector('.vehicle-filters');
        if (!form) return;

        const formData = new FormData(form);
        const params = new URLSearchParams();
        
        for (let [key, value] of formData.entries()) {
            if (value) {
                params.append(key, value);
            }
        }
        
        params.append('format', format);
        
        const exportUrl = `/Vehicle/Export?${params.toString()}`;
        window.open(exportUrl, '_blank');
    }
}

// ===== INITIALIZE VEHICLE MANAGER =====
document.addEventListener('DOMContentLoaded', () => {
    window.vehicleManager = new VehicleManager();
});

// ===== GLOBAL VEHICLE FUNCTIONS =====
window.exportVehicleData = (format) => {
    if (window.vehicleManager) {
        window.vehicleManager.exportVehicleData(format);
    }
};

// ===== VEHICLE CARD ANIMATIONS =====
document.addEventListener('DOMContentLoaded', () => {
    const vehicleCards = document.querySelectorAll('.vehicle-card');
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach((entry, index) => {
            if (entry.isIntersecting) {
                setTimeout(() => {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                }, index * 100);
            }
        });
    });

    vehicleCards.forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        card.style.transition = 'all 0.6s ease';
        observer.observe(card);
    });
});

// ===== VEHICLE IMAGE LAZY LOADING =====
document.addEventListener('DOMContentLoaded', () => {
    const vehicleImages = document.querySelectorAll('.vehicle-image img[data-src]');
    
    const imageObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.removeAttribute('data-src');
                img.classList.add('fade-in');
                imageObserver.unobserve(img);
            }
        });
    });
    
    vehicleImages.forEach(img => imageObserver.observe(img));
});

// ===== VEHICLE FILTER ANIMATIONS =====
document.addEventListener('DOMContentLoaded', () => {
    const filterForm = document.querySelector('.vehicle-filters');
    if (!filterForm) return;

    const filterInputs = filterForm.querySelectorAll('select, input');
    
    filterInputs.forEach(input => {
        input.addEventListener('focus', () => {
            input.parentElement.classList.add('focused');
        });
        
        input.addEventListener('blur', () => {
            input.parentElement.classList.remove('focused');
        });
    });
});