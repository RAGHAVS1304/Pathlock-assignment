import React, { useEffect, useState } from "react";
import api from "./api";

type Task = { id: string; description: string; isCompleted: boolean; createdAt: string };
export default function App() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [desc, setDesc] = useState("");

  const load = async () => { const res = await api.get<Task[]>("/tasks"); setTasks(res.data); };
  useEffect(() => { load(); }, []);

  const add = async () => {
    if (!desc.trim()) return;
    await api.post("/tasks", { description: desc });
    setDesc(""); await load();
  };
  const toggle = async (id:string) => { await api.put(`/tasks/${id}/toggle`); await load(); };
  const del = async (id:string) => { await api.delete(`/tasks/${id}`); await load(); };

  return (
    <div className="container mx-auto p-4">
      <h1 className="text-2xl mb-4">Task Manager</h1>
      <div className="mb-4">
        <input value={desc} onChange={e=>setDesc(e.target.value)} placeholder="Task description" />
        <button onClick={add}>Add</button>
      </div>
      <ul>
        {tasks.map(t => (
          <li key={t.id}>
            <input type="checkbox" checked={t.isCompleted} onChange={()=>toggle(t.id)} />
            <span style={{textDecoration: t.isCompleted ? "line-through" : undefined}}> {t.description}</span>
            <button onClick={()=>del(t.id)}>Delete</button>
          </li>
        ))}
      </ul>
    </div>
  );
}
