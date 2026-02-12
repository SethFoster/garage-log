"use client";
import React, { useEffect, useMemo, useState } from "react";
import { ModItem } from "../types";

export default function Dashboard() {
  const [mods, setMods] = useState<ModItem[]>([]);
  const [statusFilter, setStatusFilter] = useState<string>("All");
  const [categoryFilter, setCategoryFilter] = useState<string>("All");
  const [selectedCarId, setSelectedCarId] = useState<number | null>(null);

  useEffect(() => {
    const load = async () => {
      try {
        const base = (process.env.NEXT_PUBLIC_API_URL ?? "").replace(/\/$/, "");
        const url = base ? `${base}/mods` : `/mods`;
        const res = await fetch(url);
        const data = await res.json();
        // map backend casing to frontend-friendly
        const mapped = data.map((m: any) => ({
          id: m.id,
          name: m.name,
          category: m.category,
          cost: Number(m.cost || 0),
          status: m.status || "Planned",
          notes: m.notes || "",
          link: m.link || "",
          createdDate: m.createdDate,
          completedDate: m.completedDate,
          carId: m.carId ?? null,
          car: m.car ?? null,
          photoPath: m.photoPath ?? null,
          receiptPath: m.receiptPath ?? null,
        }));
        setMods(mapped);
      } catch (e) {
        console.error(e);
      }
    };
    load();
  }, []);

  const cars = useMemo(() => {
    const byId = new Map<number, any>();
    mods.forEach((m) => {
      if (m.car) byId.set(m.car.id, m.car);
    });
    return [{ id: -1, nickname: "All Cars" }, ...Array.from(byId.values())];
  }, [mods]);

  const filtered = useMemo(() => {
    return mods.filter((m) => {
      if (statusFilter !== "All" && m.status !== statusFilter) return false;
      if (categoryFilter !== "All" && m.category !== categoryFilter) return false;
      if (selectedCarId && selectedCarId !== -1 && m.carId !== selectedCarId) return false;
      return true;
    });
  }, [mods, statusFilter, categoryFilter, selectedCarId]);

  const totals = useMemo(() => {
    const totalCount = mods.length;
    const completedCount = mods.filter((m) => m.status === "Complete").length;
    const totalSpent = mods
      .filter((m) => m.status === "Complete")
      .reduce((s, m) => s + (m.cost || 0), 0);
    const totalPlanned = mods
      .filter((m) => m.status === "Planned")
      .reduce((s, m) => s + (m.cost || 0), 0);
    return { totalCount, completedCount, totalSpent, totalPlanned };
  }, [mods]);

  const percentComplete = totals.totalCount ? Math.round((totals.completedCount / totals.totalCount) * 100) : 0;

  const categories = useMemo(() => {
    const s = new Set<string>();
    mods.forEach((m) => s.add(m.category || "Uncategorized"));
    return ["All", ...Array.from(s)];
  }, [mods]);

  const statuses = ["All", "Planned", "In Progress", "Complete"];

  return (
    <div className="min-h-screen bg-[#1e1e1e] text-zinc-50 p-6">
      <div className="max-w-6xl mx-auto">
        <header className="mb-6 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 className="text-2xl font-semibold">Garage Log</h1>
            <p className="text-sm text-zinc-400">Build log & mod tracker</p>
          </div>

          <div className="flex gap-3 items-center">
            <select
              className="rounded-md border px-3 py-2 bg-white dark:bg-[#111]"
              value={selectedCarId ?? -1}
              onChange={(e) => setSelectedCarId(Number(e.target.value) === -1 ? null : Number(e.target.value))}
            >
              {cars.map((c: any) => (
                <option key={c.id} value={c.id}>
                  {c.nickname ?? `${c.make ?? ""} ${c.model ?? ""}`}
                </option>
              ))}
            </select>

            <div className="bg-[#252526] rounded-md px-4 py-2">
              <div className="text-xs text-zinc-400">Total spent</div>
              <div className="font-semibold">${totals.totalSpent.toFixed(2)}</div>
            </div>

            <div className="bg-[#252526] rounded-md px-4 py-2">
              <div className="text-xs text-zinc-400">Planned</div>
              <div className="font-semibold">${totals.totalPlanned.toFixed(2)}</div>
            </div>

            <div className="bg-[#252526] rounded-md px-4 py-2">
              <div className="text-xs text-zinc-400">Completed</div>
              <div className="font-semibold">{totals.completedCount}</div>
            </div>
          </div>
        </header>

        <section className="mb-6">
          <div className="bg-[#252526] p-4 rounded-md">
            <div className="mb-2 text-sm text-zinc-300">Build Progress</div>
            <div className="w-full bg-[#2d2d2d] h-3 rounded-full overflow-hidden">
              <div className="h-3 bg-emerald-500" style={{ width: `${percentComplete}%` }} />
            </div>
            <div className="mt-2 text-xs text-zinc-400">{percentComplete}% complete</div>
          </div>
        </section>

        <section className="mb-6 flex gap-3 items-center">
          <label className="text-sm text-zinc-300">Status</label>
          <select className="rounded-md border px-3 py-2 bg-[#252526] text-zinc-200" value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
            {statuses.map((s) => (
              <option key={s} value={s}>
                {s}
              </option>
            ))}
          </select>

          <label className="text-sm">Category</label>
          <select className="rounded-md border px-3 py-2 bg-[#252526] text-zinc-200" value={categoryFilter} onChange={(e) => setCategoryFilter(e.target.value)}>
            {categories.map((c) => (
              <option key={c} value={c}>
                {c}
              </option>
            ))}
          </select>
        </section>

        <section>
          <div className="bg-[#1f1f1f] rounded-md overflow-hidden">
            <table className="w-full text-left">
              <thead className="text-xs text-zinc-500 uppercase bg-zinc-100 dark:bg-[#0b0b0b]">
                <tr>
                  <th className="px-4 py-3">Mod / Part</th>
                  <th className="px-4 py-3">Category</th>
                  <th className="px-4 py-3">Cost</th>
                  <th className="px-4 py-3">Status</th>
                  <th className="px-4 py-3">Notes</th>
                  <th className="px-4 py-3">Dates</th>
                </tr>
              </thead>
              <tbody>
                {filtered.map((m) => (
                  <tr key={m.id} className="border-t border-zinc-800">
                    <td className="px-4 py-3">
                      <div className="font-medium text-zinc-100">{m.name}</div>
                      <div className="text-xs text-zinc-400">{m.car ? m.car.nickname ?? `${m.car.make} ${m.car.model}` : "—"}</div>
                    </td>
                    <td className="px-4 py-3 text-zinc-200">{m.category}</td>
                    <td className="px-4 py-3 text-zinc-200">${(m.cost || 0).toFixed(2)}</td>
                    <td className="px-4 py-3 text-zinc-200">{m.status}</td>
                    <td className="px-4 py-3 text-sm text-zinc-400">{m.notes?.slice(0, 120)}</td>
                    <td className="px-4 py-3 text-sm text-zinc-400">
                      <div>{new Date(m.createdDate).toLocaleDateString()}</div>
                      <div>{m.completedDate ? new Date(m.completedDate).toLocaleDateString() : "-"}</div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {filtered.length === 0 && <div className="p-6 text-center text-zinc-500">No mods match the filters</div>}
          </div>
        </section>
      </div>
    </div>
  );
}
