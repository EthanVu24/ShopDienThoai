const Sidebar = [
    {
        title: 'Quản lý sản phẩm',
        icon: '<i class="fa-solid fa-mobile-screen-button"></i>',
        subMenu: [
            {
                title: 'Sản phẩm',
                path: 'product'
            }
        ]
    },
    {
        title: 'Quản lý khách hàng',
        icon: '<i class="fa-solid fa-users"></i>',
        subMenu: [
            {
                title: 'Khách hàng',
                path: 'client'
            }
        ]
    },
    {
        title: 'Quản lý đơn hàng',
        icon: '<i class="fa-solid fa-cart-shopping"></i>',
        subMenu: [
            {
                title: 'Đơn hàng',
                path: 'order'
            }
        ]
    },
    {
        title: 'Quản lý tài khoản',
        icon: '<i class="fa-solid fa-user"></i>',
        subMenu: [
            {
                title: 'Tài khoản',
                path: 'account'
            }
        ]
    },
    {
        title: 'Quản lý chung',
        icon: '<i class="fa-solid fa-grip"></i>',
        subMenu: [
            {
                title: 'Hãng sản xuất',
                path: 'manufacturer',
            },
            {
                title: 'Nhà cung cấp',
                path: 'supplier'
            }
        ]
    },
]

const sidebarList = document.getElementById('sidebar-list');

sidebarList.innerHTML = Sidebar.map((item) => `
    <div class="item">
        <div class="sub-item d-flex align-items-center justify-content-between px-4 py-4">
            <div class="d-flex align-items-center">
                ${item.icon}
                <span class="ps-2">${item.title}</span>
            </div>
            <i class="fa-solid fa-chevron-down"></i>
        </div>
        <div class="sub-menu bg-secondary">
            ${item.subMenu.length > 0 ? `
                ${item.subMenu.map(subItem => `
                    <div>
                        <a href="/admin/${subItem.path}" class="text-white flex-grow-1 ps-5 py-3 d-block">
                            <i class="fa-solid fa-folder-open"></i>
                            <span class="ps-1">${subItem.title}</span>
                        </a>
                    </div>
                `).join('')}
            ` : ''}
        </div>
    </div>
    `
).join('')