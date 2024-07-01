<!-- AccountController.vue -->
<template>
    <div class="bg-white w-3/5 m-auto rounded-xl shadow-xl mt-12">
        <header class="flex justify-between border-b border-slate-300 px-8">
            <div class="items-center gap-4">
                <router-link to="/">
                    <img src="@/assets/logo1.png" alt="Logo" />
                    <div>
                        <h2 class="text-xl font-bold uppercase">ТГК-2 Энергосбыт</h2>
                        <p class="text-slate-400 mb-4">Конвертации/Транзакции/Отчеты</p>
                    </div>
                </router-link>
            </div>

            <ul class="flex items-center gap-10">
                <button @click="openReportWindow" class="flex gap-2 border border-slate-300 cursor-pointer
                        text-gray-600 hover:text-black rounded-lg p-2 bg-sky-100 shadow-lg shadow-indigo-400/40">
                    <img src="@/assets/reports.gif" alt="Cart" />
                    <p
                        class="flex items-center gap-2 text-center p-2 text-slate-500 text-lg font-bold hover:text-black">
                        Создать отчет</p>
                </button>
            </ul>
        </header>

        <div class="flex justify-between items-center border-b border-slate-300 px-80 py-5">
            <button @click="createAccount('RUB')"
                class="flex gap-2 border border-slate-300 cursor-pointer text-gray-600 hover:text-black rounded-lg p-2 bg-sky-100 shadow-lg shadow-indigo-400/40">
                Создать счет в RUB
            </button>
            <button @click="createAccount('MNT')"
                class="flex gap-2 border border-slate-300 cursor-pointer text-gray-600 hover:text-black rounded-lg p-2 bg-sky-100 shadow-lg shadow-indigo-400/40">
                Создать счет в MNT
            </button>
        </div>

        <div v-if="accounts.length === 0" class="flex items-center justify-center py-20">
            <p
                class="bg-slate-300 text-slate-500 text-center text-3xl font-bold border-2 border-slate-300 shadow-md px-20 py-5 rounded-xl">
                У вас нет активных счетов</p>
        </div>

        <div v-else class="p-8 flex justify-between">
            <div class="w-1/2 pr-4">
                <p class="flex justify-center text-xl font-bold mb-4">Счета в RUB</p>
                <div v-if="accounts.filter(acc => acc.currency === 'RUB').length === 0"
                    class="flex items-center justify-center py-20">
                    <p
                        class="bg-slate-300 text-slate-500 text-center text-2xl font-bold border-2 border-slate-300 shadow-md px-20 py-5 rounded-xl">
                        У вас нет активных счетов в RUB</p>
                </div>
                <transition-group name="list" tag="div">
                    <div v-for="(account, index) in accounts.filter(acc => acc.currency === 'RUB')" :key="account.id"
                        class="border-2 border-slate-300 bg-slate-100 text-gray-700 bg-sky-100 shadow-lg shadow-indigo-400/40 p-3 mb-4 rounded-lg shadow">
                        <div>
                            <p>Счет: №{{ index + 1 }}</p>
                            <p>Account ID: {{ account.id }}</p>
                            <p>Валюта: {{ account.currency }}</p>
                            <p>Баланс: {{ formatBalance(account.balance, 'RUB') }}</p>
                        </div>
                        <div
                            class="flex flex-inline items-end justify-center gap-5 py-1 text-lg text-gray-500 font-bold text-center border-t-2 border-slate-300">
                            <button @click="convertCurrency(account.id, 'MNT')" v-if="account.id">
                                <li class="flex items-center cursor-pointer hover:text-black">
                                    <span>Конвертация</span>
                                </li>
                            </button>
                            <button @click="openTransactionWindow(account.id)">
                                <li class="flex items-center cursor-pointer hover:text-black">
                                    <span>Транзакция</span>
                                </li>
                            </button>
                        </div>
                    </div>
                </transition-group>
            </div>

            <div class="border-r-2 border-slate-300"></div>

            <div class="w-1/2 pl-4">
                <p class="flex justify-center text-xl font-bold mb-4">Счета в MNT</p>
                <div v-if="accounts.filter(acc => acc.currency === 'MNT').length === 0"
                    class="flex items-center justify-center py-20">
                    <p
                        class="bg-slate-300 text-sky-500 text-center text-2xl font-bold border-2 border-slate-300 shadow-md px-20 py-5 rounded-xl">
                        У вас нет активных счетов в MNT</p>
                </div>
                <transition-group name="list" tag="div">
                    <div v-for="(account, index) in accounts.filter(acc => acc.currency === 'MNT')" :key="account.id"
                        class="border-2 border-sky-300 bg-sky-100 text-gray-700 bg-sky-100 shadow-lg shadow-indigo-400/40 p-3 mb-4 rounded-lg shadow">
                        <div>
                            <p>Счет: №{{ index + 1 }}</p>
                            <p>Account ID: {{ account.id }}</p>
                            <p>Валюта: {{ account.currency }}</p>
                            <p>Баланс: {{ formatBalance(account.balance, 'MNT') }}</p>
                        </div>
                        <div
                            class="flex flex-inline items-end justify-center gap-5 py-1 text-lg text-gray-500 font-bold text-center border-t-2 border-slate-300">
                            <button @click="convertCurrency(account.id, 'RUB')" v-if="account.id">
                                <li class="flex items-center cursor-pointer hover:text-black">
                                    <span>Конвертация</span>
                                </li>
                            </button>
                            <button @click="openTransactionWindow(account.id)">
                                <li class="flex items-center cursor-pointer hover:text-black">
                                    <span>Транзакция</span>
                                </li>
                            </button>
                        </div>
                    </div>
                </transition-group>
            </div>
        </div>

        <!-- Транзакции -->
        <div v-if="showTransactionWindow" class="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center">
            <div class="bg-white p-5 rounded-lg shadow-lg w-1/5 relative">
                <button @click="closeTransactionWindow"
                    class="absolute top-2 right-2 bg-none border-none text-2xl cursor-pointer">
                    <img src="@/assets/close.png" alt="Close"
                        class="mt-4 w-6 h-6 cursor-pointer hover:rotate-90 hover:scale-125 duration-150 transform hover:-translate-y-1 hover:scale-110 " />
                </button>
                <h2 class="text-xl font-bold mb-4">Транзакция</h2>
                <form @submit.prevent="submitTransaction">
                    <div class="mb-4">
                        <label for="fromAccount" class="block text-gray-700">Отправитель:</label>
                        <p
                            class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm p-2">
                            Валюта: {{ transaction.fromAccountCurrency }} | Баланс: {{
                                transaction.fromAccountBalance }}
                            | № П/С: {{ transaction.fromAccountId }}
                        </p>
                    </div>
                    <div class="mb-4">
                        <label for="transferType" class="block text-gray-700">Тип перевода:</label>
                        <select v-model="transferType"
                            class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm">
                            <option value="internal">Внутренний</option>
                            <option value="external">Внешний</option>
                        </select>
                    </div>
                    <div v-if="transferType === 'internal'" class="mb-4">
                        <label for="toAccount" class="block text-gray-700">Счет получателя:</label>
                        <select v-model="transaction.toAccountId"
                            class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm">
                            <option v-for="account in filteredAccountsTransaction" :key="account.id"
                                :value="account.id">
                                Валюта: {{ account.currency }} | Баланс: {{ account.balance }} | № П/С: {{
                                    account.id
                                }}
                            </option>
                        </select>
                    </div>
                    <div v-if="transferType === 'external'" class="mb-4">
                        <label for="externalAccount" class="block text-gray-700">Счет получателя:</label>
                        <input type="text" v-model="transaction.externalAccount" required
                            class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
                    </div>
                    <div class="mb-4">
                        <label for="amount" class="block text-gray-700">Сумма:</label>
                        <input type="number" v-model="transaction.amount" step="0.01" required
                            class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
                    </div>
                    <button type="submit"
                        class="w-full bg-indigo-600 text-white py-2 rounded-md hover:bg-indigo-700">Перевести</button>
                </form>
            </div>
        </div>

        <!-- Отчеты -->
        <div v-if="showReportWindow" class="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center">
            <div class="bg-white p-5 rounded-lg shadow-lg w-2/5 relative">
                <button @click="closeReportWindow"
                    class="absolute top-2 right-2 bg-none border-none text-2xl cursor-pointer">
                    <img src="@/assets/close.png" alt="Close"
                        class="mt-4 w-6 h-6 cursor-pointer hover:rotate-90 hover:scale-125 duration-150 transform hover:-translate-y-1 hover:scale-110 " />
                </button>
                <h2 class="text-xl font-bold mb-4">Отчеты по транзакциям</h2>
                <form @submit.prevent="fetchReports">
                    <div class="mb-4">
                        <label for="startDate" class="block text-gray-700">Дата начала:</label>
                        <input type="date" v-model="reportFilters.startDate"
                            class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
                    </div>
                    <div class="mb-4">
                        <label for="endDate" class="block text-gray-700">Дата окончания:</label>
                        <input type="date" v-model="reportFilters.endDate"
                            class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
                    </div>
                    <div class="mb-4">
                        <label for="currency" class="block text-gray-700">Валюта:</label>
                        <select v-model="reportFilters.currency"
                            class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm">
                            <option value="">Все</option>
                            <option value="RUB">RUB</option>
                            <option value="MNT">MNT</option>
                        </select>
                    </div>
                    <div class="mb-4">
                        <label for="fromAccountId" class="block text-gray-700">Счет-отправитель:</label>
                        <input type="number" v-model="reportFilters.fromAccountId"
                            class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm">
                    </div>
                    <div class="mb-4">
                        <label for="toAccountId" class="block text-gray-700">Счет-получатель:</label>
                        <input type="number" v-model="reportFilters.toAccountId"
                            class="mt-1 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
                    </div>
                    <button type="submit"
                        class="w-full bg-indigo-600 text-white py-2 rounded-md hover:bg-indigo-700">Показать
                        отчет</button>
                </form>
            </div>
        </div>

        <!-- Вывод таблицы отчетов  -->
        <div v-if="showReportsTable"
            class="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-start overflow-y-auto">
            <div class="bg-white p-5 rounded-lg shadow-lg w-2/5 relative">
                <button @click="closeReportsTable"
                    class="absolute top-2 right-2 bg-none border-none text-2xl cursor-pointer">
                    <img src="@/assets/close.png" alt="Close"
                        class="mt-4 w-6 h-6 cursor-pointer hover:rotate-90 hover:scale-125 duration-150 transform hover:-translate-y-1 hover:scale-110 " />
                </button>
                <h2 class="text-xl text-center font-bold mb-4">Таблица отчетов</h2>
                <div v-if="reports.length > 0" class="mt-4">
                    <table class="min-w-full bg-white border-collapse border border-gray-300">
                        <thead>
                            <tr>
                                <th class="py-2 border border-gray-300 bg-gray-200 font-bold">Дата</th>
                                <th class="py-2 border border-gray-300 bg-gray-200 font-bold">Валюта</th>
                                <th class="py-2 border border-gray-300 bg-gray-200 font-bold">Сумма</th>
                                <th class="py-2 border border-gray-300 bg-gray-200 font-bold">Отправитель</th>
                                <th class="py-2 border border-gray-300 bg-gray-200 font-bold">Получатель</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="report in filteredReports" :key="report.id"
                                class="even:bg-gray-100 hover:bg-gray-200">
                                <td class="py-2 border border-gray-300 text-center">{{ formatDate(report.date) }}</td>
                                <td class="py-2 border border-gray-300 text-center">{{ report.currency }}</td>
                                <td class="py-2 border border-gray-300 text-center">{{ report.amount }}</td>
                                <td class="py-2 border border-gray-300 text-center">{{ report.fromAccountId }}</td>
                                <td class="py-2 border border-gray-300 text-center">{{ report.toAccountId }}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div v-else class="mt-4">
                    <p class="text-gray-700 text-center text-2xl font-bold">Нет данных для отображения</p>
                </div>
            </div>
        </div>

    </div>
</template>

<script>
export default {
    name: "AccountController",
    data() {
        return {
            accounts: [],
            showTransactionWindow: false,
            showReportWindow: false,
            showReportsTable: false,
            transaction: {
                fromAccountId: '',
                toAccountId: '',
                externalAccount: '',
                amount: '',
                currency: ''
            },
            reportFilters: {
                startDate: '',
                endDate: '',
                currency: '',
                fromAccountId: '',
                toAccountId: '',
                transactionType: ''
            },
            reports: [],
            transferType: 'internal'
        };
    },
    computed: {
        filteredAccountsTransaction() {
            const fromAccount = this.accounts.find(account => account.id === this.transaction.fromAccountId);
            if (fromAccount) {
                return this.accounts.filter(account => account.currency === fromAccount.currency && account.id !== fromAccount.id);
            }
            return [];
        },

        filteredReports() {
            return this.reports.filter(report => {
                if (this.reportFilters.currency && this.reportFilters.currency !== '') {
                    if (this.reportFilters.currency !== 'Все' && report.currency !== this.reportFilters.currency) {
                        return false;
                    }
                }

                if (this.reportFilters.fromAccountId && this.reportFilters.fromAccountId !== '') {
                    if (this.reportFilters.fromAccountId !== 'Все' && report.fromAccountId !== this.reportFilters.fromAccountId) {
                        return false;
                    }
                }

                if (this.reportFilters.toAccountId && this.reportFilters.toAccountId !== '') {
                    if (this.reportFilters.toAccountId !== 'Все' && report.toAccountId !== this.reportFilters.toAccountId) {
                        return false;
                    }
                }

                return true;
            });
        },
    },
    methods: {
        getToken() {
            const token = sessionStorage.getItem('token');
            if (!token) {
                console.log('Ошибка получения токена при авторизации');
                return null;
            }
            return token;
        },
        fetchAccounts() {
            const token = this.getToken();
            if (!token) return;

            fetch('http://localhost:5157/Account/account/getAccounts', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            })
                .then(response => response.json())
                .then(data => {
                    this.accounts = data;
                })
                .catch(error => {
                    console.error('Ошибка получения счетов:', error);
                });
        },
        createAccount(currency) {
            if (this.accounts.length >= 5) {
                alert('Вы не можете создать более 5 аккаунтов.');
                return;
            }
            fetch(`http://localhost:5157/Account/account/create?currency=${currency}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${sessionStorage.getItem('token')}`
                }
            })
                .then(response => response.json())
                .then(() => {
                    this.fetchAccounts();
                })
                .catch(error => {
                    console.error('Ошибка создания счета:', error);
                });
        },
        convertCurrency(id, targetCurrency) {
            fetch(`http://localhost:5157/Account/account/convert?accountId=${id}&targetCurrency=${targetCurrency}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${sessionStorage.getItem('token')}`
                }
            })
                .then(response => response.json())
                .then(() => {
                    this.fetchAccounts();
                })
                .catch(error => {
                    console.error('Ошибка конвертации:', error);
                });
        },
        externalTransactionMoney(fromAccountId, externalAccountId, amount) {
            const fromAccount = this.accounts.find(acc => acc.id === fromAccountId);
            if (!fromAccount) {
                alert('Неверно указан счет отправителя');
                return;
            }
            if (fromAccount.balance < amount) {
                alert('Недостаточно средств на счете отправителя');
                return;
            }

            fetch(`http://localhost:5157/Account/account/transaction?fromAccountId=${fromAccountId}&toAccountId=${externalAccountId}&amount=${amount}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${sessionStorage.getItem('token')}`
                },
                body: JSON.stringify({ fromAccountId, toAccountId: externalAccountId, amount })
            })
                .then(response => response.json())
                .then(() => {
                    this.fetchAccounts();
                })
                .catch(error => {
                    console.error('Ошибка перевода:', error);
                });
        },
        internalTransactionMoney(fromAccountId, internalAccountId, amount) {
            const fromAccount = this.accounts.find(acc => acc.id === fromAccountId);
            if (!fromAccount) {
                alert('Неверно указан счет отправителя');
                return;
            }
            if (fromAccount.balance < amount) {
                alert('Недостаточно средств на счете отправителя');
                return;
            }

            fetch(`http://localhost:5157/Account/account/transaction?fromAccountId=${fromAccountId}&toAccountId=${internalAccountId}&amount=${amount}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${sessionStorage.getItem('token')}`
                },
                body: JSON.stringify({ fromAccountId, toAccountId: internalAccountId, amount })
            })
                .then(response => response.json())
                .then(() => {
                    this.fetchAccounts();
                })
                .catch(error => {
                    console.error('Ошибка перевода:', error);
                });
        },
        fetchReports() {
            const { startDate, endDate, currency, fromAccountId, toAccountId, transactionType } = this.reportFilters;
            const queryParams = new URLSearchParams({
                startDate,
                endDate,
                currency: currency === 'Все' ? '' : currency,
                fromAccountId: fromAccountId === 'Все' ? '' : fromAccountId,
                toAccountId: toAccountId === 'Все' ? '' : toAccountId,
                transactionType: transactionType === 'Все' ? '' : transactionType
            }).toString();

            fetch(`http://localhost:5157/Report/report?${queryParams}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${sessionStorage.getItem('token')}`
                }
            })
                .then(response => response.json())
                .then(data => {
                    this.reports = data;
                    this.showReportsTable = true;
                })
                .catch(error => {
                    console.error('Ошибка получения отчетов:', error);
                });
        },
        formatDate(value) {
            if (value) {
                const date = new Date(value);
                const day = String(date.getDate()).padStart(2, '0');
                const month = String(date.getMonth() + 1).padStart(2, '0');
                const year = date.getFullYear();
                const hours = String(date.getHours()).padStart(2, '0');
                const minutes = String(date.getMinutes()).padStart(2, '0');
                const seconds = String(date.getSeconds()).padStart(2, '0');
                return `${day}.${month}.${year} ${hours}:${minutes}:${seconds}`;
            }
            return value;
        },
        submitTransaction() {
            const { fromAccountId, toAccountId, amount, externalAccount } = this.transaction;
            if (!fromAccountId || !amount || (this.transferType === 'internal' && !toAccountId)
                || (this.transferType === 'external' && !externalAccount)) {
                alert('Вы ввели не все данные');
                return;
            }

            if (this.transferType === 'internal') {
                this.internalTransactionMoney(fromAccountId, toAccountId, amount);
            } else {
                this.externalTransactionMoney(fromAccountId, externalAccount, amount);
            }
            this.closeTransactionWindow();
        },
        formatBalance(balance, currency) {
            if (currency === 'RUB') {
                const rubles = Math.floor(balance);
                const kopecks = Math.round((balance - rubles) * 100);
                return `${rubles} руб. ${kopecks} коп.`; //рубли и копейки
            } else if (currency === 'MNT') {
                const tugriks = Math.floor(balance);
                const mungu = Math.round((balance - tugriks) * 100);
                return `${tugriks} ₮ ${mungu} м`; //тугрики и мунгу
            }
            return balance;
        },
        openTransactionWindow(accountId) {
            const fromAccount = this.accounts.find(account => account.id === accountId);
            if (fromAccount) {
                this.transaction.fromAccountId = fromAccount.id;
                this.transaction.fromAccountCurrency = fromAccount.currency;
                this.transaction.fromAccountBalance = fromAccount.balance;
            }
            this.showTransactionWindow = true;
        },
        closeTransactionWindow() {
            this.showTransactionWindow = false;
        },
        openReportWindow() {
            this.showReportWindow = true;
        },
        closeReportWindow() {
            this.showReportWindow = false;
        },
        closeReportsTable() {
            this.showReportsTable = false;
        },
        handleKeyPress(event) {
            if (event.key === 'Escape') {
                if (this.showTransactionWindow) {
                    this.closeTransactionWindow();
                } else if (this.showReportsTable) {
                    this.closeReportsTable();
                } else if (this.showReportWindow) {
                    this.closeReportWindow();
                }
            }
        },

        isInternalTransaction(report) {
            return this.accounts.some(account => account.id === report.toAccountId);
        }
    },
    mounted() {
        document.addEventListener('keydown', this.handleKeyPress);
        this.fetchAccounts();
    },
    beforeRemoveEventHandler() {
        document.removeEventListener('keydown', this.handleKeyPress);
    }
};
</script>

<style scoped>
.list-enter-active,
.list-leave-active {
    transition: all 0.5s ease;
}

.list-enter,
.list-leave-to {
    opacity: 0;
    transform: scale(0.9);
}
</style>
