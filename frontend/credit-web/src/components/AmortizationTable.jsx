const money = new Intl.NumberFormat('es-ES', { style: 'currency', currency: 'USD' });
export default function AmortizationTable({ simulation }) {
	if (!simulation) return null;
	return <div className="table-wrap"><table><thead><tr><th>Mes</th><th>Cuota</th><th>Interés</th><th>Capital</th><th>Saldo</th></tr></thead><tbody>{simulation.installments?.map((item) => <tr key={item.number}><td>{item.number}</td><td>{money.format(item.payment)}</td><td>{money.format(item.interest)}</td><td>{money.format(item.principal)}</td><td>{money.format(item.balance)}</td></tr>)}</tbody><tfoot><tr><th>Totales</th><th>{money.format(simulation.totalPaid)}</th><th>{money.format(simulation.totalInterest)}</th><th>{money.format(simulation.amount)}</th><th>{money.format(0)}</th></tr></tfoot></table></div>;
}
